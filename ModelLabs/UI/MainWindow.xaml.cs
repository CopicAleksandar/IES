using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private GDA gda;
        private ModelResourcesDesc modelResourcesDesc = new ModelResourcesDesc();
        private Dictionary<ModelCode, string> propertiesDesc = new Dictionary<ModelCode, string>();
        private Dictionary<ModelCode, string> propertiesDescExtended = new Dictionary<ModelCode, string>();
        private Dictionary<ModelCode, string> propertiesDescRelated = new Dictionary<ModelCode, string>();

        private List<long> longGids = new List<long>();
        private long selectedGID = -1;
        private long selectedDMSType = -1;
        private long selectedGIDRelated = -1;
        private long selectedRelProp = -1;

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                gda = new GDA();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "GDA Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            foreach (DMSType dmsType in Enum.GetValues(typeof(DMSType)))
            {
                if (dmsType == DMSType.MASK_TYPE)
                    continue;

                ModelCode dmsTypesModelCode = modelResourcesDesc.GetModelCodeFromType(dmsType);

                DMSTypes.Items.Add(dmsTypesModelCode);
                gda.GetExtentValues(dmsTypesModelCode, new List<ModelCode> { ModelCode.IDOBJ_GID }, null).ForEach(g => longGids.Add(g));
            }

            longGids.ForEach(g =>
            {
                GIDs.Items.Add(string.Format("{0:X16}", g));
                GIDsRelated.Items.Add(string.Format("{0:X16}", g));
            });

            selectedGID = -1;
            selectedDMSType = -1;
        }

        #region Populate properties

        private void PopulateProperties(StackPanel propertiesPanel, Dictionary<ModelCode, string> propContainer, DMSType type)
        {
            if ((long)type == -1)
                return;

            propertiesPanel.Children.Clear();

            List<ModelCode> properties = modelResourcesDesc.GetAllPropertyIds(type);

            propContainer.Clear();

            foreach (ModelCode property in properties)
            {
                propContainer.Add(property, property.ToString());

                var checkBox = new CheckBox()
                {
                    Content = string.Format("{0:X}", property.ToString()),
                };

                propertiesPanel.Children.Add(checkBox);
            }
        }

        #endregion

        #region Values tab

        private void GIDs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //long.TryParse(GIDs.SelectedItem.ToString(), out selectedGID);
            selectedGID = longGids[GIDs.SelectedIndex];
            var type = ModelCodeHelper.ExtractTypeFromGlobalId(selectedGID);
            PopulateProperties(Properties, propertiesDesc, (DMSType)type);
        }

        private void Button_Click_GetValues(object sender, RoutedEventArgs e)
        {
            if (selectedGID == -1)
                return;

            List<ModelCode> selectedProperties = new List<ModelCode>();

            foreach (var child in Properties.Children)
            {
                if (!(child is CheckBox checkBox))
                    continue;
                if (!checkBox.IsChecked.Value)
                    continue;

                foreach (KeyValuePair<ModelCode, string> keyValuePair in propertiesDesc)
                {
                    if (keyValuePair.Value.Equals(checkBox.Content))
                        selectedProperties.Add(keyValuePair.Key);
                }
            }

            ResourceDescription rd = null;
            try
            {
                rd = gda.GetValues(selectedGID, selectedProperties);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "GetValues", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (rd == null)
                return;

            var sb = new StringBuilder();
            sb.Append("Returned entity" + Environment.NewLine + Environment.NewLine);
            sb.Append($"Entity with gid: 0x{rd.Id:X16}" + Environment.NewLine);

            foreach (Property property in rd.Properties)
            {
                switch (property.Type)
                {
                    case PropertyType.Int64:
                        StringAppender.AppendLong(sb, property);
                        break;
                    case PropertyType.Float:
                        StringAppender.AppendFloat(sb, property);
                        break;
                    case PropertyType.String:
                        StringAppender.AppendString(sb, property);
                        break;
                    case PropertyType.Reference:
                        StringAppender.AppendReference(sb, property);
                        break;
                    case PropertyType.ReferenceVector:
                        StringAppender.AppendReferenceVector(sb, property);
                        break;
                    case PropertyType.DateTime:
                        StringAppender.AppendDateTime(sb, property);
                        break;
                    case PropertyType.Enum:
                        StringAppender.AppendEnum(sb, property);
                        break;
                    
                    default:
                        sb.Append($"\t{property.Id}: {property.PropertyValue.LongValue}{Environment.NewLine}");
                        break;
                }
            }

            Values.Clear();
            Values.AppendText(sb.ToString());
        }

        #endregion

        #region ExtentValues tab

        private void DMSTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedDMSType = (long)DMSTypes.SelectedItem;
            PopulateProperties(PropertiesExtent, propertiesDescExtended, ModelCodeHelper.GetTypeFromModelCode((ModelCode)selectedDMSType));
        }

        private void Button_Click_GetExtentValues(object sender, RoutedEventArgs e)
        {
            if (selectedDMSType == -1)
                return;

            List<ModelCode> selectedProperties = new List<ModelCode>();

            foreach (var child in PropertiesExtent.Children)
            {
                if (!(child is CheckBox checkBox))
                    continue;
                if (!checkBox.IsChecked.Value)
                    continue;

                foreach (KeyValuePair<ModelCode, string> keyValuePair in propertiesDescExtended)
                {
                    if (keyValuePair.Value.Equals(checkBox.Content))
                        selectedProperties.Add(keyValuePair.Key);
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("Returned entities" + Environment.NewLine + Environment.NewLine);

            try
            {
                gda.GetExtentValues((ModelCode)selectedDMSType, selectedProperties, sb);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "GetValues", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ValuesExtent.Clear();
            ValuesExtent.AppendText(sb.ToString());
        }

        #endregion

        #region RelatedValues tab

        private void RelatedGIDs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RelationalProps.Items.Clear();
            RelationalTypes.Items.Clear();
            PropertiesRelated.Children.Clear();

            selectedGIDRelated = longGids[GIDsRelated.SelectedIndex];
            short type = ModelCodeHelper.ExtractTypeFromGlobalId(selectedGIDRelated);
            List<ModelCode> properties = modelResourcesDesc.GetAllPropertyIds((DMSType)type);

            foreach (ModelCode property in properties)
            {
                var prop = new Property(property);
                if (prop.Type != PropertyType.Reference && prop.Type != PropertyType.ReferenceVector)
                    continue;

                RelationalProps.Items.Add(property);
            }
        }

        private void RelationalProps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RelationalProps.SelectedItem == null)
                return;

            RelationalTypes.Items.Clear();

            selectedRelProp = (long)RelationalProps.SelectedItem;

            var targetEntity = GetTypeFromReferenceModelCode(selectedRelProp);

            RelationalTypes.Items.Add(targetEntity);

            PopulateProperties(PropertiesRelated, propertiesDescRelated, targetEntity);
        }
        
        
        private DMSType GetTypeFromReferenceModelCode(long modelCode)
        {
            var rd = gda.GetValues(selectedGIDRelated, new List<ModelCode>() { (ModelCode)modelCode });
            var prop = rd.GetProperty((ModelCode)modelCode);

            long gid = -1;
            if (prop.IsCompatibleWith(PropertyType.ReferenceVector))
            {
                if (prop.AsReferences().Count > 0)
                    gid = prop.AsReferences()[0];
            }
            else
            {
                gid = prop.AsReference();
            }

            if (gid == -1)
                return DMSType.MASK_TYPE;

            var targetEntity = (DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(gid);
            return targetEntity;
        }

        private void Button_Click_GetRelatedValues(object sender, RoutedEventArgs e)
        {
            if (selectedRelProp == -1)
                return;

            List<ModelCode> selectedProperties = new List<ModelCode>();

            foreach (var child in PropertiesRelated.Children)
            {
                if (child is CheckBox checkBox && checkBox.IsChecked.Value)
                {
                    foreach (KeyValuePair<ModelCode, string> keyValuePair in propertiesDescRelated)
                    {
                        if (keyValuePair.Value.Equals(checkBox.Content))
                        {
                            selectedProperties.Add(keyValuePair.Key);
                        }
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("Returned entities" + Environment.NewLine + Environment.NewLine);

            // Property is of reference type, but value not set so skip it.
            var type = GetTypeFromReferenceModelCode(selectedRelProp);
            if (type == DMSType.MASK_TYPE)
                return;

            var association = new Association((ModelCode)selectedRelProp, modelResourcesDesc.GetModelCodeFromType(type));
            List<long> gids = gda.GetRelatedValues(selectedGIDRelated, selectedProperties, association, sb);

            ValuesRelated.Clear();
            ValuesRelated.AppendText(sb.ToString());
        }

        #endregion
        
    }
}
