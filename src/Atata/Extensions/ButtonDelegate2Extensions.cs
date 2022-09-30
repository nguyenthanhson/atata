﻿namespace Atata
{
    public static class ButtonDelegate2Extensions
    {
        public static Button<TNavigateTo, TOwner> GetControl<TNavigateTo, TOwner>(this ButtonDelegate<TNavigateTo, TOwner> clickable)
            where TNavigateTo : PageObject<TNavigateTo>
            where TOwner : PageObject<TOwner>
            =>
            (Button<TNavigateTo, TOwner>)UIComponentResolver.GetControlByDelegate<TOwner>(clickable);

        public static TOwner Click<TNavigateTo, TOwner>(this ButtonDelegate<TNavigateTo, TOwner> clickable)
            where TNavigateTo : PageObject<TNavigateTo>
            where TOwner : PageObject<TOwner>
            =>
            clickable.GetControl().Click();

        public static TOwner Hover<TNavigateTo, TOwner>(this ButtonDelegate<TNavigateTo, TOwner> clickable)
            where TNavigateTo : PageObject<TNavigateTo>
            where TOwner : PageObject<TOwner>
            =>
            clickable.GetControl().Hover();

        public static TOwner Focus<TNavigateTo, TOwner>(this ButtonDelegate<TNavigateTo, TOwner> clickable)
            where TNavigateTo : PageObject<TNavigateTo>
            where TOwner : PageObject<TOwner>
            =>
            clickable.GetControl().Focus();

        public static TOwner DoubleClick<TNavigateTo, TOwner>(this ButtonDelegate<TNavigateTo, TOwner> clickable)
            where TNavigateTo : PageObject<TNavigateTo>
            where TOwner : PageObject<TOwner>
            =>
            clickable.GetControl().DoubleClick();

        public static TOwner RightClick<TNavigateTo, TOwner>(this ButtonDelegate<TNavigateTo, TOwner> clickable)
            where TNavigateTo : PageObject<TNavigateTo>
            where TOwner : PageObject<TOwner>
            =>
            clickable.GetControl().RightClick();

        public static TOwner ScrollTo<TNavigateTo, TOwner>(this ButtonDelegate<TNavigateTo, TOwner> clickable)
            where TNavigateTo : PageObject<TNavigateTo>
            where TOwner : PageObject<TOwner>
            =>
            clickable.GetControl().ScrollTo();

        public static bool IsEnabled<TNavigateTo, TOwner>(this ButtonDelegate<TNavigateTo, TOwner> clickable)
            where TNavigateTo : PageObject<TNavigateTo>
            where TOwner : PageObject<TOwner>
            =>
            clickable.GetControl().IsEnabled;

        public static bool Exists<TNavigateTo, TOwner>(this ButtonDelegate<TNavigateTo, TOwner> clickable, SearchOptions options = null)
            where TNavigateTo : PageObject<TNavigateTo>
            where TOwner : PageObject<TOwner>
            =>
            clickable.GetControl().Exists(options);

        public static bool Missing<TNavigateTo, TOwner>(this ButtonDelegate<TNavigateTo, TOwner> clickable, SearchOptions options = null)
            where TNavigateTo : PageObject<TNavigateTo>
            where TOwner : PageObject<TOwner>
            =>
            clickable.GetControl().Missing(options);

        public static ValueProvider<string, TOwner> Content<TNavigateTo, TOwner>(this ButtonDelegate<TNavigateTo, TOwner> clickable)
            where TNavigateTo : PageObject<TNavigateTo>
            where TOwner : PageObject<TOwner>
            =>
            clickable.GetControl().Content;

        public static UIComponentVerificationProvider<Control<TOwner>, TOwner> Should<TNavigateTo, TOwner>(this ButtonDelegate<TNavigateTo, TOwner> clickable)
            where TNavigateTo : PageObject<TNavigateTo>
            where TOwner : PageObject<TOwner>
            =>
            clickable.GetControl().Should;

        public static UIComponentVerificationProvider<Control<TOwner>, TOwner> ExpectTo<TNavigateTo, TOwner>(this ButtonDelegate<TNavigateTo, TOwner> clickable)
            where TNavigateTo : PageObject<TNavigateTo>
            where TOwner : PageObject<TOwner>
            =>
            clickable.GetControl().ExpectTo;

        public static UIComponentVerificationProvider<Control<TOwner>, TOwner> WaitTo<TNavigateTo, TOwner>(this ButtonDelegate<TNavigateTo, TOwner> clickable)
            where TNavigateTo : PageObject<TNavigateTo>
            where TOwner : PageObject<TOwner>
            =>
            clickable.GetControl().WaitTo;
    }
}
