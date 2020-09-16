using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CIM.Model;

namespace FTN.ESI.SIMES.CIM.CIMAdapter.Importer
{
    public static class IES1Converter
    {
        #region Populate ResourceDescription

        public static void PopulateIdentifiedObjectProperties(FTN.IdentifiedObject cimIdentifiedObject, ResourceDescription rd)
        {
            if ((cimIdentifiedObject != null) && (rd != null))
            {
                if (cimIdentifiedObject.MRIDHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.IDOBJ_MRID, cimIdentifiedObject.MRID));
                }
                if (cimIdentifiedObject.NameHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.IDOBJ_NAME, cimIdentifiedObject.Name));
                }
                if (cimIdentifiedObject.AliasNameHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.IDOBJ_ALIASNAME, cimIdentifiedObject.AliasName));
                }
            }
        }

        public static void PopulatePowerSystemResourceProperties(FTN.PowerSystemResource cimPowerSystemResource, ResourceDescription rd)
        {
            if ((cimPowerSystemResource != null) && (rd != null))
            {
                IES1Converter.PopulateIdentifiedObjectProperties(cimPowerSystemResource, rd);                
            }
        }

        public static void PopulateEquipmentProperties(Equipment cimEquipment, ResourceDescription rd)
        {
            if ((cimEquipment != null) && (rd != null))
            {
                IES1Converter.PopulatePowerSystemResourceProperties(cimEquipment, rd);

                if (cimEquipment.AggregateHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.EQUIPMENT_AGGREGATE, cimEquipment.Aggregate));
                }
                if (cimEquipment.NormallyInServiceHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.EQUIPMENT_NORMALLYINSERVICE, cimEquipment.NormallyInService));
                }
            }
        }

        public static void PopulateConductingEquipmentProperties(ConductingEquipment ce, ResourceDescription rd)
        {
            if ((ce != null) && (rd != null))
            {
                IES1Converter.PopulateEquipmentProperties(ce, rd);                
            }
        }

        public static void PopulateRegulatingControlProperties(RegulatingControl rc, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if((rc != null) && (rd != null))
            {
                IES1Converter.PopulatePowerSystemResourceProperties(rc, rd);

                if (rc.DiscreteHasValue)
                    rd.AddProperty(new Property(ModelCode.REGULATING_CONTROL_DISCRETE, rc.Discrete));
                if(rc.ModeHasValue)
                    rd.AddProperty(new Property(ModelCode.REGULATING_CONTROL_MODE, (short)GetRegulatingControlModeKind(rc.Mode)));
                if(rc.MonitoredPhaseHasValue)
                    rd.AddProperty(new Property(ModelCode.REGULATING_CONTROL_MONITORED_PHASE, (short)GetDMSPhaseCode(rc.MonitoredPhase)));
                if (rc.TargetRangeHasValue)
                    rd.AddProperty(new Property(ModelCode.REGULATING_CONTROL_TARGET_RANGE, rc.TargetRange));
                if (rc.TargetValueHasValue)
                    rd.AddProperty(new Property(ModelCode.REGULATING_CONTROL_TARGET_VALUE, rc.TargetValue));
                if (rc.TerminalHasValue)
                {
                    long gid = importHelper.GetMappedGID(rc.Terminal.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(rc.GetType().ToString()).Append(" rdfID = \"")
                            .Append(rc.ID);
                        report.Report.Append("\" - Failed to set reference to Terminal: rdfID \"")
                            .Append(rc.Terminal.ID).AppendLine(" \" is not mapped to GID!");
                    }

                    rd.AddProperty(new Property(ModelCode.REGULATING_CONTROL_TERMINAL, gid));
                }
            }   
        }

        public static void PopulateBasicIntervalScheduleProperties(FTN.BasicIntervalSchedule cimBasicIntervalSchedule, ResourceDescription rd)
        {
            if((cimBasicIntervalSchedule != null) && rd != null)
            {
                IES1Converter.PopulateIdentifiedObjectProperties(cimBasicIntervalSchedule, rd);

                if (cimBasicIntervalSchedule.StartTimeHasValue)
                    rd.AddProperty(new Property(ModelCode.BASIC_INT_SCHED_STARTTIME, cimBasicIntervalSchedule.StartTime));
                if (cimBasicIntervalSchedule.Value1MultiplierHasValue)
                    rd.AddProperty(new Property(ModelCode.BASIC_INT_SCHED_VAL1MUL, (short)GetUnitMultiplier(cimBasicIntervalSchedule.Value1Multiplier)));
                if (cimBasicIntervalSchedule.Value1UnitHasValue)
                    rd.AddProperty(new Property(ModelCode.BASIC_INT_SCHED_VAL1UNIT, (short)GetUnitSymbol(cimBasicIntervalSchedule.Value1Unit)));
                if (cimBasicIntervalSchedule.Value2MultiplierHasValue)
                    rd.AddProperty(new Property(ModelCode.BASIC_INT_SCHED_VAL2MUL, (short)GetUnitMultiplier(cimBasicIntervalSchedule.Value2Multiplier)));
                if (cimBasicIntervalSchedule.Value2UnitHasValue)
                    rd.AddProperty(new Property(ModelCode.BASIC_INT_SCHED_VAL2UNIT, (short)GetUnitSymbol(cimBasicIntervalSchedule.Value2Unit)));
            }          
        }

        public static void PopulateRegularIntervalSchedule(RegularIntervalSchedule regularIntervalSchedule, ResourceDescription rd)
        {
            if((regularIntervalSchedule != null) && (rd != null))
            {
                IES1Converter.PopulateBasicIntervalScheduleProperties(regularIntervalSchedule, rd);
            }
        }

        public static void PopulateSeasonDayTypeScheduleProperties(SeasonDayTypeSchedule schedule, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if((schedule != null) && (rd != null))
            {
                IES1Converter.PopulateRegularIntervalSchedule(schedule, rd);

                if (schedule.DayTypeHasValue)
                {
                    long gid = importHelper.GetMappedGID(schedule.DayType.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(schedule.GetType().ToString()).Append(" rdfID = \"").Append(schedule.ID);
                        report.Report.Append("\" - Failed to set reference to DayType: rdfID \"").Append(schedule.DayType.ID).AppendLine(" \" is not mapped to GID!");
                    }

                    rd.AddProperty(new Property(ModelCode.SEASON_DAY_TYPE_SCHEDULE_DAYTYPE, gid));

                }
            }            
        }
        
        public static void PopulateTerminalProperties(Terminal t, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if((t != null) && (rd) != null)
            {
                IES1Converter.PopulateIdentifiedObjectProperties(t, rd);

                if (t.ConductingEquipmentHasValue)
                {
                    long gid = importHelper.GetMappedGID(t.ConductingEquipment.ID);

                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(t.GetType().ToString()).Append(" rdfID = \"").Append(t.ID);
                        report.Report.Append("\" - Failed to set reference to ConductingEquipment: rdfID \"").Append(t.ConductingEquipment.ID).AppendLine(" \" is not mapped to GID!");
                    }

                    rd.AddProperty(new Property(ModelCode.TERMINAL_CONDEQ, gid));
                }
            }            
        }

        public static void PopulateRegulationScheduleProperties(RegulationSchedule rs, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if((rs != null) && (rd != null))
            {
                PopulateSeasonDayTypeScheduleProperties(rs, rd, importHelper, report);

                if (rs.RegulatingControlHasValue)
                {
                    long gid = importHelper.GetMappedGID(rs.RegulatingControl.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(rs.GetType().ToString()).Append(" rdfID = \"").Append(rs.ID);
                        report.Report.Append("\" - Failed to set reference to RegulatingControl: rdfID \"").Append(rs.RegulatingControl.ID).AppendLine(" \" is not mapped to GID!");
                    }

                    rd.AddProperty(new Property(ModelCode.REGULATION_SCHEDULE_REG_CONTROL, gid));
                }
            }            
        }

        public static void PopulateRegulatingCondEqProperties(RegulatingCondEq rc, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if((rc != null) && (rd != null))
            {
                IES1Converter.PopulateConductingEquipmentProperties(rc, rd);

                if (rc.RegulatingControlHasValue)
                {
                    long gid = importHelper.GetMappedGID(rc.RegulatingControl.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(rc.GetType().ToString()).Append(" rdfID = \"").Append(rc.ID);
                        report.Report.Append("\" - Failed to set reference to RegulatingControl: rdfID \"").Append(rc.RegulatingControl.ID).AppendLine(" \" is not mapped to GID!");
                    }

                    rd.AddProperty(new Property(ModelCode.REGULATING_CONDEQ_REG_CONTROL, gid));
                }
            }
        }

        public static void PopulateStaticVarCompensatorProperties(StaticVarCompensator sc, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if((sc != null) && (rd != null))
            {
                IES1Converter.PopulateRegulatingCondEqProperties(sc, rd, importHelper, report);
            }
        }

        public static void PopulateShuntCompensatorProperties(ShuntCompensator sc, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((sc != null) && (rd != null))
            {
                IES1Converter.PopulateRegulatingCondEqProperties(sc, rd, importHelper, report);
            }
        }

        public static void PopulateDayTypeProperties(DayType dt, ResourceDescription rd)
        {
            if((dt != null) && (rd != null))
            {
                IES1Converter.PopulateIdentifiedObjectProperties(dt, rd);
            }
        }

        public static void PopulateProperties<T>(T cimObj, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if (typeof(T) == typeof(Terminal))
                PopulateTerminalProperties(cimObj as Terminal, rd, importHelper, report);
            else if (typeof(T) == typeof(DayType))
                PopulateDayTypeProperties(cimObj as DayType, rd);
            else if (typeof(T) == typeof(RegulatingControl))
                PopulateRegulatingControlProperties(cimObj as RegulatingControl, rd, importHelper, report);
            else if (typeof(T) == typeof(RegulationSchedule))
                PopulateRegulationScheduleProperties(cimObj as RegulationSchedule, rd, importHelper, report);
            else if (typeof(T) == typeof(StaticVarCompensator))
                PopulateStaticVarCompensatorProperties(cimObj as StaticVarCompensator, rd, importHelper, report);
            else if (typeof(T) == typeof(ShuntCompensator))
                PopulateShuntCompensatorProperties(cimObj as ShuntCompensator, rd, importHelper, report);
        }

        #endregion

        #region Enums convert

        public static Common.RegulatingControlModeKind GetRegulatingControlModeKind(FTN.RegulatingControlModeKind modeKind)
        {
            switch (modeKind)
            {
                case RegulatingControlModeKind.activePower:
                    return Common.RegulatingControlModeKind.ActivePower;
                case RegulatingControlModeKind.admittance:
                    return Common.RegulatingControlModeKind.Admittance;
                case RegulatingControlModeKind.currentFlow:
                    return Common.RegulatingControlModeKind.CurrentFlow;
                case RegulatingControlModeKind.@fixed:
                    return Common.RegulatingControlModeKind.Fixed;
                case RegulatingControlModeKind.powerFactor:
                    return Common.RegulatingControlModeKind.PowerFactor;
                case RegulatingControlModeKind.reactivePower:
                    return Common.RegulatingControlModeKind.ReactivePower;
                case RegulatingControlModeKind.temperature:
                    return Common.RegulatingControlModeKind.Temperature;
                case RegulatingControlModeKind.timeScheduled:
                    return Common.RegulatingControlModeKind.TimeScheduled;
                case RegulatingControlModeKind.voltage:
                    return Common.RegulatingControlModeKind.Voltage;
                default:
                    throw new ArgumentOutOfRangeException(nameof(modeKind), modeKind, null);
            }
        }

        public static Common.UnitMultiplier GetUnitMultiplier(FTN.UnitMultiplier unitsM)
        {
            switch (unitsM)
            {
                case FTN.UnitMultiplier.G:
                    return Common.UnitMultiplier.G;
                case FTN.UnitMultiplier.M:
                    return Common.UnitMultiplier.M;
                case FTN.UnitMultiplier.T:
                    return Common.UnitMultiplier.T;
                case FTN.UnitMultiplier.c:
                    return Common.UnitMultiplier.c;
                case FTN.UnitMultiplier.d:
                    return Common.UnitMultiplier.d;
                case FTN.UnitMultiplier.k:
                    return Common.UnitMultiplier.k;
                case FTN.UnitMultiplier.m:
                    return Common.UnitMultiplier.m;
                case FTN.UnitMultiplier.micro:
                    return Common.UnitMultiplier.micro;
                case FTN.UnitMultiplier.n:
                    return Common.UnitMultiplier.n;
                case FTN.UnitMultiplier.none:
                    return Common.UnitMultiplier.none;
                case FTN.UnitMultiplier.p:
                    return Common.UnitMultiplier.p;

                default: return Common.UnitMultiplier.none;
            }
        }

        public static Common.UnitSymbol GetUnitSymbol(FTN.UnitSymbol unitSymbol)
        {
            switch (unitSymbol)
            {
                case FTN.UnitSymbol.A:
                    return Common.UnitSymbol.A;
                case FTN.UnitSymbol.F:
                    return Common.UnitSymbol.F;
                case FTN.UnitSymbol.H:
                    return Common.UnitSymbol.H;
                case FTN.UnitSymbol.Hz:
                    return Common.UnitSymbol.Hz;
                case FTN.UnitSymbol.J:
                    return Common.UnitSymbol.J;
                case FTN.UnitSymbol.N:
                    return Common.UnitSymbol.N;
                case FTN.UnitSymbol.Pa:
                    return Common.UnitSymbol.Pa;
                case FTN.UnitSymbol.S:
                    return Common.UnitSymbol.S;
                case FTN.UnitSymbol.V:
                    return Common.UnitSymbol.V;
                case FTN.UnitSymbol.VA:
                    return Common.UnitSymbol.VA;
                case FTN.UnitSymbol.VAh:
                    return Common.UnitSymbol.VAh;
                case FTN.UnitSymbol.VAr:
                    return Common.UnitSymbol.VAr;
                case FTN.UnitSymbol.VArh:
                    return Common.UnitSymbol.VArh;
                case FTN.UnitSymbol.W:
                    return Common.UnitSymbol.W;
                case FTN.UnitSymbol.Wh:
                    return Common.UnitSymbol.Wh;
                case FTN.UnitSymbol.deg:
                    return Common.UnitSymbol.deg;
                case FTN.UnitSymbol.degC:
                    return Common.UnitSymbol.degC;
                case FTN.UnitSymbol.g:
                    return Common.UnitSymbol.g;
                case FTN.UnitSymbol.h:
                    return Common.UnitSymbol.h;
                case FTN.UnitSymbol.m:
                    return Common.UnitSymbol.m;
                case FTN.UnitSymbol.m2:
                    return Common.UnitSymbol.m2;
                case FTN.UnitSymbol.m3:
                    return Common.UnitSymbol.m3;
                case FTN.UnitSymbol.min:
                    return Common.UnitSymbol.min;
                case FTN.UnitSymbol.none:
                    return Common.UnitSymbol.none;
                case FTN.UnitSymbol.ohm:
                    return Common.UnitSymbol.ohm;
                case FTN.UnitSymbol.rad:
                    return Common.UnitSymbol.rad;
                case FTN.UnitSymbol.s:
                    return Common.UnitSymbol.s;

                default:
                    return Common.UnitSymbol.none;
            }
        }

        public static Common.PhaseCode GetDMSPhaseCode(FTN.PhaseCode phase)
        {
            switch (phase)
            {
                case FTN.PhaseCode.A:
                    return Common.PhaseCode.A;
                case FTN.PhaseCode.AB:
                    return Common.PhaseCode.AB;
                case FTN.PhaseCode.ABC:
                    return Common.PhaseCode.ABC;
                case FTN.PhaseCode.ABCN:
                    return Common.PhaseCode.ABCN;
                case FTN.PhaseCode.ABN:
                    return Common.PhaseCode.ABN;
                case FTN.PhaseCode.AC:
                    return Common.PhaseCode.AC;
                case FTN.PhaseCode.ACN:
                    return Common.PhaseCode.ACN;
                case FTN.PhaseCode.AN:
                    return Common.PhaseCode.AN;
                case FTN.PhaseCode.B:
                    return Common.PhaseCode.B;
                case FTN.PhaseCode.BC:
                    return Common.PhaseCode.BC;
                case FTN.PhaseCode.BCN:
                    return Common.PhaseCode.BCN;
                case FTN.PhaseCode.BN:
                    return Common.PhaseCode.BN;
                case FTN.PhaseCode.C:
                    return Common.PhaseCode.C;
                case FTN.PhaseCode.CN:
                    return Common.PhaseCode.CN;
                case FTN.PhaseCode.N:
                    return Common.PhaseCode.N;
                case FTN.PhaseCode.s12N:
                    return Common.PhaseCode.ABN;
                case FTN.PhaseCode.s1N:
                    return Common.PhaseCode.AN;
                case FTN.PhaseCode.s2N:
                    return Common.PhaseCode.BN;
                default: return Common.PhaseCode.Unknown;
            }
        }

        #endregion
    }
}
