// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using TypeTooling.Types.PredefinedTypes;
using TypeTooling.Xaml.CustomAttributes;
using TypeTooling.Xaml.TypeConverters.Uwp;
using TypeTooling.Xaml.TypeConverters.UwpWpf;

namespace TypeTooling.Xaml.Uwp
{
    /// <summary>
    /// DO NOT MODIFY MANUALLY.
    /// This class has been generated from UwpHostAttributeTable.xml using Meta.exe.
    /// </summary>
    internal class UwpHostAttributeTableBuilder : DesignAttributeTableBuilder {
        private readonly XamlTypeToolingProvider _xamlTypeToolingProvider;

        internal UwpHostAttributeTableBuilder(XamlTypeToolingProvider xamlTypeToolingProvider)
        {
            _xamlTypeToolingProvider = xamlTypeToolingProvider;

            this.AddWindows_Foundation_PointAttributes();
            this.AddWindows_Foundation_RectAttributes();
            this.AddWindows_Foundation_SizeAttributes();
            this.AddWindows_UI_ColorAttributes();
            this.AddWindows_UI_Text_FontStretchAttributes();
            this.AddWindows_UI_Text_FontStyleAttributes();
            this.AddWindows_UI_Text_FontWeightAttributes();
            this.AddWindows_UI_Xaml_Controls_AppBarButtonAttributes();
            this.AddWindows_UI_Xaml_Controls_AppBarToggleButtonAttributes();
            this.AddWindows_UI_Xaml_Controls_AutoSuggestBoxAttributes();
            this.AddWindows_UI_Xaml_Controls_CalendarDatePickerAttributes();
            this.AddWindows_UI_Xaml_Controls_CalendarViewAttributes();
            this.AddWindows_UI_Xaml_Controls_CanvasAttributes();
            this.AddWindows_UI_Xaml_Controls_ContentPresenterAttributes();
            this.AddWindows_UI_Xaml_Controls_ControlAttributes();
            this.AddWindows_UI_Xaml_Controls_DatePickerAttributes();
            this.AddWindows_UI_Xaml_Controls_FontIconAttributes();
            this.AddWindows_UI_Xaml_Controls_PasswordBoxAttributes();
            this.AddWindows_UI_Xaml_Controls_PivotAttributes();
            this.AddWindows_UI_Xaml_Controls_PivotItemAttributes();
            this.AddWindows_UI_Xaml_Controls_RichEditBoxAttributes();
            this.AddWindows_UI_Xaml_Controls_RichTextBlockAttributes();
            this.AddWindows_UI_Xaml_Controls_SymbolIconAttributes();
            this.AddWindows_UI_Xaml_Controls_TextBlockAttributes();
            this.AddWindows_UI_Xaml_Controls_TextBoxAttributes();
            this.AddWindows_UI_Xaml_Controls_TimePickerAttributes();
            this.AddWindows_UI_Xaml_CornerRadiusAttributes();
            this.AddWindows_UI_Xaml_Documents_InlineAttributes();
            this.AddWindows_UI_Xaml_Documents_TextElementAttributes();
            this.AddWindows_UI_Xaml_DurationAttributes();
            this.AddWindows_UI_Xaml_FrameworkElementAttributes();
            this.AddWindows_UI_Xaml_FrameworkTemplateAttributes();
            this.AddWindows_UI_Xaml_GridLengthAttributes();
            this.AddWindows_UI_Xaml_Media_Animation_KeySplineAttributes();
            this.AddWindows_UI_Xaml_Media_Animation_KeyTimeAttributes();
            this.AddWindows_UI_Xaml_Media_Animation_RepeatBehaviorAttributes();
            this.AddWindows_UI_Xaml_Media_BrushAttributes();
            this.AddWindows_UI_Xaml_Media_CacheModeAttributes();
            this.AddWindows_UI_Xaml_Media_DoubleCollectionAttributes();
            this.AddWindows_UI_Xaml_Media_FontFamilyAttributes();
            this.AddWindows_UI_Xaml_Media_GeometryAttributes();
            this.AddWindows_UI_Xaml_Media_ImageSourceAttributes();
            this.AddWindows_UI_Xaml_Media_MatrixAttributes();
            this.AddWindows_UI_Xaml_Media_Media3D_Matrix3DAttributes();
            this.AddWindows_UI_Xaml_Media_PointCollectionAttributes();
            this.AddWindows_UI_Xaml_Media_TransformAttributes();
            this.AddWindows_UI_Xaml_PropertyPathAttributes();
            this.AddWindows_UI_Xaml_StyleAttributes();
            this.AddWindows_UI_Xaml_ThicknessAttributes();
            this.AddWindows_UI_Xaml_UIElementAttributes();
        }

        private void AddWindows_Foundation_PointAttributes()
        {
            AddCallback("Windows.Foundation.Point", delegate (DesignAttributeCallbackBuilder builder)
            {
                //builder.AddCustomAttributes(new TypeConverterAttribute(new System.Windows.PointConverter()));
            });
        }

        private void AddWindows_Foundation_RectAttributes()
        {
            AddCallback("Windows.Foundation.Rect", delegate (DesignAttributeCallbackBuilder builder)
            {
                //builder.AddCustomAttributes(new TypeConverterAttribute(new System.Windows.RectConverter()));
            });
        }

        private void AddWindows_Foundation_SizeAttributes()
        {
            AddCallback("Windows.Foundation.Size", delegate (DesignAttributeCallbackBuilder builder)
            {
                //builder.AddCustomAttributes(new TypeConverterAttribute(new System.Windows.SizeConverter()));
            });
        }

        private void AddWindows_UI_ColorAttributes()
        {
            AddCallback("Windows.UI.Color", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(new CustomLiteralParserAttribute(new UwpWpfColorCustomLiteralParser(_xamlTypeToolingProvider)));
            });
        }

        private void AddWindows_UI_Text_FontStretchAttributes()
        {
            AddCallback("Windows.UI.Text.FontStretch", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(new TypeConverterAttribute(new UwpWpfFontStretchConverter(_xamlTypeToolingProvider)));
            });
        }

        private void AddWindows_UI_Text_FontStyleAttributes()
        {
            AddCallback("Windows.UI.Text.FontStyle", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(new TypeConverterAttribute(new UwpWpfFontStyleConverter(_xamlTypeToolingProvider)));
            });
        }

        private void AddWindows_UI_Text_FontWeightAttributes()
        {
            AddCallback("Windows.UI.Text.FontWeight", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(new TypeConverterAttribute(new UwpWpfFontWeightConverter(_xamlTypeToolingProvider)));
            });
        }

        private void AddWindows_UI_Xaml_Controls_AppBarButtonAttributes()
        {
            AddCallback("Windows.UI.Xaml.Controls.AppBarButton", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("Icon", new TypeConverterAttribute(new UwpSymbolIconConverter(_xamlTypeToolingProvider)));
            });
        }

        private void AddWindows_UI_Xaml_Controls_AppBarToggleButtonAttributes()
        {
            AddCallback("Windows.UI.Xaml.Controls.AppBarToggleButton", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("Icon", new TypeConverterAttribute(new UwpSymbolIconConverter(_xamlTypeToolingProvider)));
            });
        }

        private void AddWindows_UI_Xaml_Controls_AutoSuggestBoxAttributes()
        {
            AddCallback("Windows.UI.Xaml.Controls.AutoSuggestBox", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("QueryIcon", new TypeConverterAttribute(new UwpSymbolIconConverter(_xamlTypeToolingProvider)));
            });
        }

        private void AddWindows_UI_Xaml_Controls_CalendarDatePickerAttributes()
        {
            AddCallback("Windows.UI.Xaml.Controls.CalendarDatePicker", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("CalendarIdentifier", new TypeConverterAttribute(new UwpCalendarIdentifierConverter()));
                builder.AddCustomAttributes("DateFormat", new TypeConverterAttribute(new UwpDateFormatConverter()));
                builder.AddCustomAttributes("DayOfWeekFormat", new TypeConverterAttribute(new UwpDayOfWeekFormatConverter()));
            });
        }

        private void AddWindows_UI_Xaml_Controls_CalendarViewAttributes()
        {
            AddCallback("Windows.UI.Xaml.Controls.CalendarView", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("CalendarIdentifier", new TypeConverterAttribute(new UwpCalendarIdentifierConverter()));
                builder.AddCustomAttributes("DayOfWeekFormat", new TypeConverterAttribute(new UwpDayOfWeekFormatConverter()));
            });
        }

        private void AddWindows_UI_Xaml_Controls_CanvasAttributes()
        {
            AddCallback("Windows.UI.Xaml.Controls.Canvas", delegate (DesignAttributeCallbackBuilder builder)
            {
                //builder.AddCustomAttributes("Left", new TypeConverterAttribute(new System.Windows.LengthConverter()));
                //builder.AddCustomAttributes("Top", new TypeConverterAttribute(new System.Windows.LengthConverter()));
            });
        }

        private void AddWindows_UI_Xaml_Controls_ContentPresenterAttributes()
        {
            AddCallback("Windows.UI.Xaml.Controls.ContentPresenter", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("FontSize", new TypeConverterAttribute(new UwpFontSizeConverter()));
            });
        }

        private void AddWindows_UI_Xaml_Controls_ControlAttributes()
        {
            AddCallback("Windows.UI.Xaml.Controls.Control", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("FontSize", new TypeConverterAttribute(new UwpFontSizeConverter()));
            });
        }

        private void AddWindows_UI_Xaml_Controls_DatePickerAttributes()
        {
            AddCallback("Windows.UI.Xaml.Controls.DatePicker", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("CalendarIdentifier", new TypeConverterAttribute(new UwpCalendarIdentifierConverter()));
                builder.AddCustomAttributes("DayFormat", new TypeConverterAttribute(new UwpDayFormatConverter()));
                builder.AddCustomAttributes("MonthFormat", new TypeConverterAttribute(new UwpMonthFormatConverter()));
                builder.AddCustomAttributes("YearFormat", new TypeConverterAttribute(new UwpYearFormatConverter()));
            });
        }

        private void AddWindows_UI_Xaml_Controls_FontIconAttributes()
        {
            AddCallback("Windows.UI.Xaml.Controls.FontIcon", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("FontSize", new TypeConverterAttribute(new UwpFontSizeConverter()));
            });
        }

        private void AddWindows_UI_Xaml_Controls_PasswordBoxAttributes()
        {
            AddCallback("Windows.UI.Xaml.Controls.PasswordBox", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("InputScope", new TypeConverterAttribute(new UwpWpfInputScopeConverter(_xamlTypeToolingProvider)));
            });
        }

        private void AddWindows_UI_Xaml_Controls_PivotAttributes()
        {
            AddCallback("Windows.UI.Xaml.Controls.Pivot", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("Title", new TreatAsTypeAttribute(UITextType.Instance));
            });
        }

        private void AddWindows_UI_Xaml_Controls_PivotItemAttributes()
        {
            AddCallback("Windows.UI.Xaml.Controls.PivotItem", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("Header", new TreatAsTypeAttribute(UITextType.Instance));
            });
        }

        private void AddWindows_UI_Xaml_Controls_RichEditBoxAttributes()
        {
            AddCallback("Windows.UI.Xaml.Controls.RichEditBox", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("InputScope", new TypeConverterAttribute(new UwpWpfInputScopeConverter(_xamlTypeToolingProvider)));
            });
        }

        private void AddWindows_UI_Xaml_Controls_RichTextBlockAttributes()
        {
            AddCallback("Windows.UI.Xaml.Controls.RichTextBlock", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("FontSize", new TypeConverterAttribute(new UwpFontSizeConverter()));
            });
        }

        private void AddWindows_UI_Xaml_Controls_SymbolIconAttributes()
        {
            AddCallback("Windows.UI.Xaml.Controls.SymbolIcon", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(new TypeConverterAttribute(new UwpSymbolIconConverter(_xamlTypeToolingProvider)));
                builder.AddCustomAttributes("Symbol", new TypeConverterAttribute(new UwpSymbolIconConverter(_xamlTypeToolingProvider)));
            });
        }

        private void AddWindows_UI_Xaml_Controls_TextBlockAttributes()
        {
            AddCallback("Windows.UI.Xaml.Controls.TextBlock", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("FontSize", new TypeConverterAttribute(new UwpFontSizeConverter()));
            });
        }

        private void AddWindows_UI_Xaml_Controls_TextBoxAttributes()
        {
            AddCallback("Windows.UI.Xaml.Controls.TextBox", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("FontSize", new TypeConverterAttribute(new UwpFontSizeConverter()));
                builder.AddCustomAttributes("InputScope", new TypeConverterAttribute(new UwpWpfInputScopeConverter(_xamlTypeToolingProvider)));
            });
        }

        private void AddWindows_UI_Xaml_Controls_TimePickerAttributes()
        {
            AddCallback("Windows.UI.Xaml.Controls.TimePicker", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("ClockIdentifier", new TypeConverterAttribute(new UwpClockIdentifierConverter()));
            });
        }

        private void AddWindows_UI_Xaml_CornerRadiusAttributes()
        {
            AddCallback("Windows.UI.Xaml.CornerRadius", delegate (DesignAttributeCallbackBuilder builder)
            {
                //builder.AddCustomAttributes(new TypeConverterAttribute(new System.Windows.CornerRadiusConverter()));
            });
        }

        private void AddWindows_UI_Xaml_Documents_InlineAttributes()
        {
            AddCallback("Windows.UI.Xaml.Documents.Inline", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("FontSize", new TypeConverterAttribute(new UwpFontSizeConverter()));
            });
        }

        private void AddWindows_UI_Xaml_Documents_TextElementAttributes()
        {
            AddCallback("Windows.UI.Xaml.Documents.TextElement", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("FontSize", new TypeConverterAttribute(new UwpFontSizeConverter()));
            });
        }

        private void AddWindows_UI_Xaml_DurationAttributes()
        {
            AddCallback("Windows.UI.Xaml.Duration", delegate (DesignAttributeCallbackBuilder builder)
            {
                //builder.AddCustomAttributes(new TypeConverterAttribute(new System.Windows.DurationConverter()));
            });
        }

        private void AddWindows_UI_Xaml_FrameworkElementAttributes()
        {
            AddCallback("Windows.UI.Xaml.FrameworkElement", delegate (DesignAttributeCallbackBuilder builder)
            {
                //builder.AddCustomAttributes("Height", new TypeConverterAttribute(new System.Windows.LengthConverter()));
                builder.AddCustomAttributes("Tag", new TreatAsTypeAttribute(StringType.Instance));
                //builder.AddCustomAttributes("Width", new TypeConverterAttribute(new System.Windows.LengthConverter()));
            });
        }

        private void AddWindows_UI_Xaml_FrameworkTemplateAttributes()
        {
            AddCallback("Windows.UI.Xaml.FrameworkTemplate", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(new TypeConverterAttribute(new System.ComponentModel.TypeConverter()));
            });
        }

        private void AddWindows_UI_Xaml_GridLengthAttributes()
        {
            AddCallback("Windows.UI.Xaml.GridLength", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(new CustomLiteralParserAttribute(new UwpGridLengthParser(_xamlTypeToolingProvider)));
            });
        }

        private void AddWindows_UI_Xaml_Media_Animation_KeySplineAttributes()
        {
            AddCallback("Windows.UI.Xaml.Media.Animation.KeySpline", delegate (DesignAttributeCallbackBuilder builder)
            {
                //builder.AddCustomAttributes(new TypeConverterAttribute(new System.Windows.KeySplineConverter()));
            });
        }

        private void AddWindows_UI_Xaml_Media_Animation_KeyTimeAttributes()
        {
            AddCallback("Windows.UI.Xaml.Media.Animation.KeyTime", delegate (DesignAttributeCallbackBuilder builder)
            {
                //builder.AddCustomAttributes(new TypeConverterAttribute(new System.Windows.KeyTimeConverter()));
            });
        }

        private void AddWindows_UI_Xaml_Media_Animation_RepeatBehaviorAttributes()
        {
            AddCallback("Windows.UI.Xaml.Media.Animation.RepeatBehavior", delegate (DesignAttributeCallbackBuilder builder)
            {
                //builder.AddCustomAttributes(new TypeConverterAttribute(new System.Windows.Media.Animation.RepeatBehaviorConverter()));
            });
        }

        private void AddWindows_UI_Xaml_Media_BrushAttributes()
        {
            AddCallback("Windows.UI.Xaml.Media.Brush", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(new CustomLiteralParserAttribute(new UwpWpfBrushParser(_xamlTypeToolingProvider)));
            });
        }

        private void AddWindows_UI_Xaml_Media_CacheModeAttributes()
        {
            AddCallback("Windows.UI.Xaml.Media.CacheMode", delegate (DesignAttributeCallbackBuilder builder)
            {
                //builder.AddCustomAttributes(new TypeConverterAttribute(new System.Windows.Media.CacheModeConverter()));
            });
        }

        private void AddWindows_UI_Xaml_Media_DoubleCollectionAttributes()
        {
            AddCallback("Windows.UI.Xaml.Media.DoubleCollection", delegate (DesignAttributeCallbackBuilder builder)
            {
                //builder.AddCustomAttributes(new TypeConverterAttribute(new System.Windows.Media.DoubleCollectionConverter()));
            });
        }

        private void AddWindows_UI_Xaml_Media_FontFamilyAttributes()
        {
            AddCallback("Windows.UI.Xaml.Media.FontFamily", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(new TreatAsTypeAttribute(StringType.Instance));
            });
        }

        private void AddWindows_UI_Xaml_Media_GeometryAttributes()
        {
            AddCallback("Windows.UI.Xaml.Media.Geometry", delegate (DesignAttributeCallbackBuilder builder)
            {
                //builder.AddCustomAttributes(new TypeConverterAttribute(new System.Windows.Media.GeometryConverter()));
            });
        }

        private void AddWindows_UI_Xaml_Media_ImageSourceAttributes()
        {
            AddCallback("Windows.UI.Xaml.Media.ImageSource", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(new TypeConverterAttribute(new UwpWpfImageSourceConverter()));
            });
        }

        private void AddWindows_UI_Xaml_Media_MatrixAttributes()
        {
            AddCallback("Windows.UI.Xaml.Media.Matrix", delegate (DesignAttributeCallbackBuilder builder)
            {
                //builder.AddCustomAttributes(new TypeConverterAttribute(new System.Windows.Media.MatrixConverter()));
            });
        }

        private void AddWindows_UI_Xaml_Media_Media3D_Matrix3DAttributes()
        {
            AddCallback("Windows.UI.Xaml.Media.Media3D.Matrix3D", delegate (DesignAttributeCallbackBuilder builder)
            {
                //builder.AddCustomAttributes(new TypeConverterAttribute(new System.Windows.Media.Media3D.Matrix3DConverter()));
            });
        }

        private void AddWindows_UI_Xaml_Media_PointCollectionAttributes()
        {
            AddCallback("Windows.UI.Xaml.Media.PointCollection", delegate (DesignAttributeCallbackBuilder builder)
            {
                //builder.AddCustomAttributes(new TypeConverterAttribute(new System.Windows.Media.PointCollectionConverter()));
            });
        }

        private void AddWindows_UI_Xaml_Media_TransformAttributes()
        {
            AddCallback("Windows.UI.Xaml.Media.Transform", delegate (DesignAttributeCallbackBuilder builder)
            {
                //builder.AddCustomAttributes(new TypeConverterAttribute(new System.Windows.Media.TransformConverter()));
            });
        }

        private void AddWindows_UI_Xaml_PropertyPathAttributes()
        {
            AddCallback("Windows.UI.Xaml.PropertyPath", delegate (DesignAttributeCallbackBuilder builder)
            {
                //builder.AddCustomAttributes(new TypeConverterAttribute(new System.Windows.PropertyPathConverter()));
            });
        }

        private void AddWindows_UI_Xaml_StyleAttributes()
        {
            AddCallback("Windows.UI.Xaml.Style", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(new TypeConverterAttribute(new System.ComponentModel.TypeConverter()));
            });
        }

        private void AddWindows_UI_Xaml_ThicknessAttributes()
        {
            AddCallback("Windows.UI.Xaml.Thickness", delegate (DesignAttributeCallbackBuilder builder)
            {
                //builder.AddCustomAttributes(new TypeConverterAttribute(new System.Windows.ThicknessConverter()));
            });
        }

        private void AddWindows_UI_Xaml_UIElementAttributes()
        {
            AddCallback("Windows.UI.Xaml.UIElement", delegate (DesignAttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("CacheMode", new TypeConverterAttribute(new UwpCacheModeConverter()));
            });
        }

    }
}
