// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System.ComponentModel;

namespace TypeTooling.Xaml.TypeConverters.UwpWpf
{
    class StringConverterWithStandardValuesFromStaticList : StringConverter
    {
        private StandardValuesCollection standardValues;

        public StringConverterWithStandardValuesFromStaticList(string[] standardValues)
        {
            this.standardValues = new StandardValuesCollection(standardValues);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return this.standardValues;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }

    class UwpDayFormatConverter : StringConverterWithStandardValuesFromStaticList
    {
        private static readonly string[] commonDayFormats = 
            new string[] {
                "{day.integer}",
                "{day.integer(2)}",
                "{dayofweek.full}",
                "{dayofweek.abbreviated}",
                "{dayofweek.abbreviated(3)}",
                "{dayofweek.solo.full}",
                "{dayofweek.solo.abbreviated}",
                "{dayofweek.solo.abbreviated(3)}",
                "{day.integer} {dayofweek.full}",
                "{day.integer} {dayofweek.abbreviated}"
            };

        public UwpDayFormatConverter() : base(commonDayFormats) { }
    }

    class UwpMonthFormatConverter : StringConverterWithStandardValuesFromStaticList
    {
        private static readonly string[] commonMonthFormats = 
            new string[] {
                "{month.integer}",
                "{month.integer(2)}",
                "{month.full}",
                "{month.abbreviated}",
                "{month.abbreviated(3)}",
                "{month.solo.full}",
                "{month.solo.abbreviated}",
                "{month.solo.abbreviated(3)}",
                "{month.integer} {month.full}",
                "{month.integer} {month.abbreviated}"
            };
        public UwpMonthFormatConverter() : base(UwpMonthFormatConverter.commonMonthFormats) { }
    }

    class UwpYearFormatConverter : StringConverterWithStandardValuesFromStaticList
    {
        private static readonly string[] commonYearFormats =
            new string[] {
                "{year.full}",
                "{year.full(2)}",
                "{year.full(4)}",
                "{year.abbreviated}",
                "{year.abbreviated(2)}",
                "{year.abbreviated(4)}"
            };

        public UwpYearFormatConverter() : base(UwpYearFormatConverter.commonYearFormats) { }
    }

    class UwpDateFormatConverter : StringConverterWithStandardValuesFromStaticList
    {
        private static readonly string[] commonDateFormats =
            new string[] {
                "{day.integer}/{month.integer}/{year.full}",
                "{day.integer} {month.full} {year.full}",
                "{month.integer}/{day.integer}/{year.full}",
                "{month.full} {day.integer}, {year.full}",
                "{year.full}/{month.integer}/{day.integer}"
            };

        public UwpDateFormatConverter() : base(UwpDateFormatConverter.commonDateFormats) { }
    }

    class UwpDayOfWeekFormatConverter : StringConverterWithStandardValuesFromStaticList
    {
        internal static readonly string[] CommonDayOfWeekFormats =
            new string[] {
                "{dayofweek.abbreviated}",
                "{dayofweek.abbreviated(2)}",
                "{dayofweek.abbreviated(3)}",
                "{dayofweek.solo.abbreviated}",
                "{dayofweek.solo.abbreviated(2)}",
                "{dayofweek.solo.abbreviated(3)}"
            };

        public UwpDayOfWeekFormatConverter() : base(UwpDayOfWeekFormatConverter.CommonDayOfWeekFormats) { }
    }

    class UwpCalendarIdentifierConverter : StringConverterWithStandardValuesFromStaticList
    {
        internal static readonly string[] CalendarIdentifiers =
            new string[] {
                "GregorianCalendar",
                "HebrewCalendar",
                "HijriCalendar",
                "JapaneseCalendar",
                "JulianCalendar",
                "KoreanCalendar",
                "TaiwanCalendar",
                "ThaiCalendar",
                "UmAlQuraCalendar"
            };

        public UwpCalendarIdentifierConverter() : base(UwpCalendarIdentifierConverter.CalendarIdentifiers) { }
    }

    class UwpClockIdentifierConverter : StringConverterWithStandardValuesFromStaticList
    {
        internal static readonly string[] ClockIdentifiers =
            new string[] {
                "12HourClock",
                "24HourClock"
            };

        public UwpClockIdentifierConverter() : base(UwpClockIdentifierConverter.ClockIdentifiers) { }
    }

    class UwpCacheModeConverter : StringConverterWithStandardValuesFromStaticList
    {
        internal static readonly string[] CommonCacheModes =
            new string[] {
                "BitmapCache"
            };

        public UwpCacheModeConverter() : base(UwpCacheModeConverter.CommonCacheModes) { }
    }
}
