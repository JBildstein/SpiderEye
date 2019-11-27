using System;
using System.Runtime.InteropServices;
using SpiderEye.Linux.Interop;

namespace SpiderEye.Linux.Native
{
    internal static partial class AppIndicator
    {
        private const string AppIndicatorNativeDll = "libappindicator3.so";

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_new", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Create(IntPtr id, IntPtr icon_name, AppIndicatorCategory category);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_new_with_path", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreateWithPath(IntPtr id, IntPtr icon_name, AppIndicatorCategory category, IntPtr icon_theme_path);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_set_status", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetStatus(IntPtr self, AppIndicatorStatus status);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_set_attention_icon", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetAttentionIcon(IntPtr self, IntPtr icon_name);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_set_attention_icon_full", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetAttentionIconFull(IntPtr self, IntPtr icon_name, IntPtr icon_desc);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_set_menu", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetMenu(IntPtr self, IntPtr menu);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_set_icon", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetIcon(IntPtr self, IntPtr icon_name);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_set_icon_full", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetIconFull(IntPtr self, IntPtr icon_name, IntPtr icon_desc);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_set_label", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetLabel(IntPtr self, IntPtr label, IntPtr guide);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_set_icon_theme_path", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetIconThemePath(IntPtr self, IntPtr icon_theme_path);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_set_ordering_index", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetOrderingIndex(IntPtr self, uint ordering_index);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_set_secondary_activate_target", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetSecondaryActivateTarget(IntPtr self, IntPtr menuitem);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_set_title", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetTitle(IntPtr self, IntPtr title);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_get_id", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetId(IntPtr self);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_get_category", CallingConvention = CallingConvention.Cdecl)]
        public static extern AppIndicatorCategory GetCategory(IntPtr self);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_get_status", CallingConvention = CallingConvention.Cdecl)]
        public static extern AppIndicatorStatus GetStatus(IntPtr self);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_get_icon", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetIcon(IntPtr self);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_get_icon_desc", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetIconDesc(IntPtr self);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_get_icon_theme_path", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetIconThemePath(IntPtr self);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_get_attention_icon", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetAttentionIcon(IntPtr self);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_get_attention_icon_desc", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetAttentionIconDesc(IntPtr self);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_get_title", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetTitle(IntPtr self);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_get_menu", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetMenu(IntPtr self);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_get_label", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetLabel(IntPtr self);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_get_label_guide", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetLabelGuide(IntPtr self);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_get_ordering_index", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint GetOrderingIndex(IntPtr self);

        [DllImport(AppIndicatorNativeDll, EntryPoint = "app_indicator_get_secondary_activate_target", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetSecondaryActivateTarget(IntPtr self);
    }
}
