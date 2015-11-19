using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace _2048_UWP
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated || e.PreviousExecutionState == ApplicationExecutionState.ClosedByUser)
                {
                    //TODO: 从之前挂起的应用程序加载状态
                    Debug.WriteLine(e.PreviousExecutionState.ToString());
                    UserData.Load();
                }

                // 将框架放在当前窗口中
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // 当导航堆栈尚未还原时，导航到第一页，
                // 并通过将所需信息作为导航参数传入来配置
                // 参数
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // 确保当前窗口处于活动状态
            Window.Current.Activate();
        }

        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            Debug.WriteLine("OnSuspending");
            UserData.Save();
            deferral.Complete();
        }
    }
    class UserData
    {
        private static UserData _userData = null;
        static UserData() { _userData = new UserData(); }
        public static UserData CurrentInstance { get { return _userData; } }

        string score = "0", best = "0"; int[,] num = new int[4, 4];

        public string Score { get { return score; } set { score = value; } }
        public string Best { get { return best; } set { best = value; } }
        public int[,] Num { get { return num; } set { num = value; } }

        public static void Load()
        {
            Debug.WriteLine("正加载数据...");
            var rs = ApplicationData.Current.RoamingSettings;
            object v = null;
            if (rs.Values.TryGetValue("score", out v)) { CurrentInstance.Score = (string)v; } else { CurrentInstance.Score = "0"; }
            if (rs.Values.TryGetValue("best", out v)) { CurrentInstance.Best = (string)v; } else { CurrentInstance.Best = "0"; }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (rs.Values.TryGetValue(i + "" + j, out v)) { CurrentInstance.Num[i, j] = (int)v; }
                    else { CurrentInstance.Num[i, j] = 0; }
                }
            }//for
        }//load

        public static void Save()
        {
            var rs = ApplicationData.Current.RoamingSettings;
            rs.Values["score"] = UserData.CurrentInstance.Score;
            rs.Values["best"] = UserData.CurrentInstance.best;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    rs.Values[i + "" + j] = CurrentInstance.Num[i, j];
                }
            }//for
        }//save
    }//class

}//namespace
