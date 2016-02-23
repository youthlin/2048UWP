using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace _2048_UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public const int LEFT = 0;
        public const int UP = 1;
        public const int RIGHT = 2;
        public const int DOWN = 3;
        Tile[,] tiles;
        Point start;
        int[,] num;
        int nth = 0;
        public MainPage()
        {
            this.InitializeComponent();
            bigmain.ManipulationStarted += _ManipulationStarted;
            bigmain.ManipulationDelta += _ManipulationDelta;
            bigmain.ManipulationCompleted += _ManipulationCompleted;
            this.Loaded += MainPage_Loaded;
        }


        #region 判断移动方向
        private void _ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e) { start = e.Position; }
        private void _ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            Point end = e.Position;
            e.Complete();
            if (Math.Abs(end.X - start.X) < 5 && Math.Abs(end.Y - start.Y) < 5)
            {
                return;
            }

            if (Math.Abs(end.X - start.X) > Math.Abs(end.Y - start.Y))
            {
                if (end.X - start.X > 0) { Move(RIGHT); }
                else { Move(LEFT); }
            }
            else
            {
                if (end.Y - start.Y > 0) { Move(DOWN); }
                else { Move(UP); }
            }
        }
        private void _ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) { }
        #endregion

        private void Page_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            Debug.WriteLine(e.OriginalKey);

            if (e.OriginalKey == Windows.System.VirtualKey.A || e.OriginalKey == Windows.System.VirtualKey.Left || e.OriginalKey == Windows.System.VirtualKey.H)
            {
                Move(LEFT);
            }
            if (e.OriginalKey == Windows.System.VirtualKey.W || e.OriginalKey == Windows.System.VirtualKey.Up || e.OriginalKey == Windows.System.VirtualKey.K)
            {
                Move(UP);
            }
            if (e.OriginalKey == Windows.System.VirtualKey.D || e.OriginalKey == Windows.System.VirtualKey.Right || e.OriginalKey == Windows.System.VirtualKey.L)
            {
                Move(RIGHT);
            }
            if (e.OriginalKey == Windows.System.VirtualKey.S || e.OriginalKey == Windows.System.VirtualKey.Down || e.OriginalKey == Windows.System.VirtualKey.J)
            {
                Move(DOWN);
            }
        }

        private async void IfGameOver()
        {
            if (isGameOver())
            {
                Debug.WriteLine("Game Over");
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "游戏结束", //标题
                    Content = "\n已无法合并更多数字\n您的得分为:" + score.Text + ",最高分为:" + best.Text + ".",//内容
                    FullSizeDesired = false,  //是否全屏展示
                    PrimaryButtonText = "新游戏",//第一个按钮内容
                    SecondaryButtonText = "返回"
                };
                btn.LostFocus -= Btn_LostFocus;
                try
                {
                    var result = await dialog.ShowAsync();
                    Debug.WriteLine("Result=" + result);
                    if (ContentDialogResult.Primary == result)
                    {
                        NewGame(nth++);
                    }
                    btn.LostFocus += Btn_LostFocus;
                }
                catch (Exception ex) { Debug.WriteLine(ex.Message); }
            }

        }
        private bool isGameOver()
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    Debug.WriteLine(i + " " + j);
                    if (tiles[i, j].Number == 0) return false;
                    if (i - 1 >= 0)
                        if (tiles[i, j].Number == tiles[i - 1, j].Number)
                            return false;
                    if (j - 1 >= 0)
                        if (tiles[i, j].Number == tiles[i, j - 1].Number) return false;
                    if (j + 1 < 4)
                        if (tiles[i, j].Number == tiles[i, j + 1].Number) return false;
                    if (i + 1 < 4)
                        if (tiles[i, j].Number == tiles[i + 1, j].Number) return false;
                }
            return true;
        }

        private void Move(int direction)
        {
            IfGameOver();
            Debug.WriteLine(direction);

            // 初始化num中间数组为全零
            num = new int[4, 4];
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    num[i, j] = 0;

            //清除没有数字的格子
            bool hasBlankMove = ClearBlank(direction);
            int i_score = int.Parse(score.Text);
            bool hasAddMove = AddNumber(direction, ref i_score);
            score.Text = i_score.ToString();
            if (i_score > int.Parse(best.Text)) { best.Text = i_score.ToString(); }

            if (hasAddMove | hasBlankMove)
            {
                // 产生1个新的数字
                Random random = new Random();
                int a = random.Next(15);
                if (a == 0)
                    a = 4;
                else
                    a = 2;
                int x = 0, y = 0;
                do
                {   // 产生[0,3]随机数
                    x = random.Next(4);
                    y = random.Next(4);
                } while (tiles[x, y].Number != 0);
                tiles[x, y].Number = a;
                //新数字出现动画
                tiles[x, y].Appera();
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Debug.Write(tiles[i, j].Number + " ");

                    //更新待保存数据
                    App.UserData.CurrentInstance.Num[i, j] = tiles[i, j].Number;
                }
                Debug.WriteLine("");
            }
            Debug.WriteLine("score = " + score.Text + " best = " + best.Text);
            App.UserData.CurrentInstance.Score = score.Text;
            App.UserData.CurrentInstance.Best = best.Text;

        }
        private bool ClearBlank(int o)
        {
            bool hasBlankMove = false;
            if (o == LEFT)
            {
                for (int i = 0; i < 4; i++)
                {
                    int t = 0;
                    for (int j = 0; j < 4; j++)
                        if (0 != tiles[i, j].Number)
                            num[i, t++] = tiles[i, j].Number;
                }
            }//LEFT

            if (o == RIGHT)
            {
                for (int i = 0; i < 4; i++)
                {
                    int t = 3;
                    for (int j = 3; j >= 0; j--)
                        if (0 != tiles[i, j].Number)
                            num[i, t--] = tiles[i, j].Number;

                }
            }// RIGHT

            if (o == UP)
            {
                for (int j = 0; j < 4; j++)
                {
                    int t = 0;
                    for (int i = 0; i < 4; i++)
                        if (0 != tiles[i, j].Number)
                            num[t++, j] = tiles[i, j].Number;
                }
            }// UP

            if (o == DOWN)
            {
                for (int j = 0; j < 4; j++)
                {
                    int t = 3;
                    for (int i = 3; i >= 0; i--)
                        if (0 != tiles[i, j].Number)
                            num[t--, j] = tiles[i, j].Number;
                }
            }//DOWN

            // 更新numbers
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    // 移除空白后与原来不相同,说明有移动
                    if (tiles[i, j].Number != num[i, j])
                        hasBlankMove = true;
                    tiles[i, j].Number = num[i, j];
                }
            }

            return hasBlankMove;
        }

        private bool AddNumber(int o, ref int s)
        {
            bool hasAddMove = false;
            if (o == LEFT)
            {
                Debug.WriteLine("←");
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (tiles[i, j].Number == tiles[i, j + 1].Number
                                && tiles[i, j].Number != 0)
                        {
                            //放大的动画
                            tiles[i, j].Zoom();

                            tiles[i, j].Number += tiles[i, j + 1].Number;
                            s += tiles[i, j].Number;
                            hasAddMove = true;
                            for (int t = j + 1; t < 3; t++)
                            {
                                tiles[i, t].Number = tiles[i, t + 1].Number;
                            }
                            tiles[i, 3].Number = 0;
                        }
                    }
                }// 每行

            }// LEFT

            if (o == RIGHT)
            {
                Debug.WriteLine("→");
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 3; j > 0; j--)
                    {
                        if (tiles[i, j].Number == tiles[i, j - 1].Number
                                && tiles[i, j].Number != 0)
                        {
                            //放大的动画
                            tiles[i, j].Zoom();

                            tiles[i, j].Number += tiles[i, j - 1].Number;
                            s += tiles[i, j].Number;
                            hasAddMove = true;
                            for (int t = j - 1; t > 0; t--)
                            {
                                tiles[i, t].Number = tiles[i, t - 1].Number;
                            }
                            tiles[i, 0].Number = 0;
                        }
                    }
                }
            }// RIGHT

            if (o == UP)
            {
                Debug.WriteLine("↑");
                for (int j = 0; j < 4; j++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (tiles[i, j].Number == tiles[i + 1, j].Number
                                && tiles[i, j].Number != 0)
                        {
                            //放大的动画
                            tiles[i, j].Zoom();

                            tiles[i, j].Number += tiles[i + 1, j].Number;
                            hasAddMove = true;
                            s += tiles[i, j].Number;
                            for (int t = i + 1; t < 3; t++)
                            {
                                tiles[t, j].Number = tiles[t + 1, j].Number;
                            }
                            tiles[3, j].Number = 0;
                        }
                    }
                }
            }// UP

            if (o == DOWN)
            {
                Debug.WriteLine("↓");
                for (int j = 0; j < 4; j++)
                {
                    for (int i = 3; i > 0; i--)
                    {
                        if (tiles[i, j].Number == tiles[i - 1, j].Number
                                && tiles[i, j].Number != 0)
                        {
                            //放大的动画
                            tiles[i, j].Zoom();

                            tiles[i, j].Number += tiles[i - 1, j].Number;
                            hasAddMove = true;
                            s += tiles[i, j].Number;
                            for (int t = i - 1; t > 0; t--)
                            {
                                tiles[t, j].Number = tiles[t - 1, j].Number;
                            }
                            tiles[0, j].Number = 0;
                        }
                    }
                }
            }//DOWN

            return hasAddMove;
        }

        private void MoveAnimate(Tile tile, int direction, int distance)
        {
            //使用代码创建动画：
            //https://msdn.microsoft.com/zh-cn/library/windows/apps/windows.ui.xaml.media.animation.storyboard.aspx

            // 创建平移动画
            TranslateTransform moveTransform = new TranslateTransform();
            //设置起始坐标
            moveTransform.X = 0;
            moveTransform.Y = 0;
            //关联对象与动画
            tile.RenderTransform = moveTransform;
            //设置动画时间
            Duration duration = new Duration(TimeSpan.FromSeconds(1));
            // 创建xy方向的动画
            DoubleAnimation myDoubleAnimationX = new DoubleAnimation();
            DoubleAnimation myDoubleAnimationY = new DoubleAnimation();
            //设置动画时间
            myDoubleAnimationX.Duration = duration;
            myDoubleAnimationY.Duration = duration;
            //创建故事板
            Storyboard moveStoryboard = new Storyboard();
            moveStoryboard.Duration = duration;
            //把xy方向的动画添加到故事板
            moveStoryboard.Children.Add(myDoubleAnimationX);
            moveStoryboard.Children.Add(myDoubleAnimationY);
            //设置xy动画的目标是平移动画
            Storyboard.SetTarget(myDoubleAnimationX, moveTransform);
            Storyboard.SetTarget(myDoubleAnimationY, moveTransform);
            //设置动画要修改的属性是X、Y坐标
            Storyboard.SetTargetProperty(myDoubleAnimationX, "X");
            Storyboard.SetTargetProperty(myDoubleAnimationY, "Y");
            //设置结束坐标
            switch (direction)
            {
                case LEFT: myDoubleAnimationX.To = -tile.ActualWidth * distance; break;
                case UP: myDoubleAnimationY.To = -tile.ActualHeight * distance; break;
                case RIGHT: myDoubleAnimationX.To = tile.ActualWidth * distance; break;
                case DOWN: myDoubleAnimationY.To = tile.ActualHeight * distance; break;
            }
            //Canvas.ZIndex Attached Property
            //https://msdn.microsoft.com/zh-cn/library/windows/apps/windows.ui.xaml.controls.canvas.zindex.aspx
            tile.SetValue(Canvas.ZIndexProperty, 999);
            //开始动画
            moveStoryboard.Begin();
        }

        private void NewGame(int times)
        {
            //首次打开游戏
            if (times == 0)
            {
                //如果有上次游戏数据
                score.Text = App.UserData.CurrentInstance.Score;
                best.Text = App.UserData.CurrentInstance.Best;
                bool all0 = true;
                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 4; j++)
                    {
                        tiles[i, j].Number = App.UserData.CurrentInstance.Num[i, j];
                        if (tiles[i, j].Number != 0) all0 = false;
                    }
                if (all0) Init();
            }
            //点击新游戏
            else Init();
        }

        private void Init()
        {
            //重置为零
            foreach (Tile t in tiles)
                t.Number = 0;
            Random random = new Random();
            //其中一个为2，另一个为2(90%)或4(10%)
            int a = 4;
            if (random.Next(0, 10) != 0) a = 2;
            int x1 = random.Next(0, 4),
                y1 = random.Next(0, 4);
            int x2, y2;
            do
            {
                x2 = random.Next(0, 4);
                y2 = random.Next(0, 4);
            } while (x1 == x2 && y1 == y2);
            tiles[x1, y1].Number = 2;
            tiles[x2, y2].Number = a;
            tiles[x1, y1].Appera();
            tiles[x2, y2].Appera();
            score.Text = "0";
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            #region Size
            //Size size = e.NewSize;
            //不能直接用e.NewSize因为手机上有状态栏(显示信号电量)而且横屏时宽度和竖屏时高度还不一样！
            //机智的我在ApplicationView上点击查看定义就看到了获取可见区域的VisibleBounds字段：
            //
            // 摘要:
            //     获取窗口（应用程序视图）的可见区。可见区域是未被镶边封闭的区域，例如状态栏和应用程序栏。
            //
            // 返回结果:
            //     窗口（应用程序视图）的可见区。
            //public Rect VisibleBounds { get; }
            Size size = new Size(Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().VisibleBounds.Width,
                                Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().VisibleBounds.Height);
            #endregion
            wrap.Width = size.Width;
            wrap.Height = size.Height;
            double up = 0.25, down = 0.75, left = 0.4, right = 0.6, logomargin = 10;

            if (Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().Orientation == Windows.UI.ViewManagement.ApplicationViewOrientation.Portrait)
            {
                //竖屏
                //竖向排列头部和主区域
                wrap.Orientation = Orientation.Vertical;
                //横向排列头部内容
                header.Orientation = Orientation.Horizontal;

                header.Height = size.Height * up;
                footer.Height = size.Height * down;
                header.Width = size.Width;
                footer.Width = size.Width;

                logo.Width = size.Width * left - logomargin;
                logo.Height = size.Height * up - logomargin;
                logo.Margin = new Thickness(logomargin);

                headerRight.Width = size.Width * right;
                headerRight.Height = size.Height * up;

                bigmain.Width = (size.Width > size.Height * down) ? (size.Height * down) : (size.Width);
                bigmain.Height = (size.Width > size.Height * down) ? (size.Height * down) : (size.Width);
                bigmain.Margin = new Thickness((size.Width - bigmain.Width) / 2,
                    (size.Height * down - bigmain.Height) / 2,
                    (size.Width - bigmain.Width) / 2,
                    (size.Height * down - bigmain.Height) / 2);
            }
            else
            {
                //横屏
                wrap.Orientation = Orientation.Horizontal;
                header.Orientation = Orientation.Vertical;

                header.Width = size.Width * up;
                footer.Width = size.Width * down;
                header.Height = size.Height;
                footer.Height = size.Height;

                logo.Height = size.Height * left - logomargin;
                logo.Width = size.Width * up - logomargin;
                logo.Margin = new Thickness(logomargin);

                headerRight.Height = size.Height * right;
                headerRight.Width = size.Width * up;

                bigmain.Height = (size.Height > size.Width * down) ? (size.Width * down) : (size.Height);
                bigmain.Width = (size.Height > size.Width * down) ? (size.Width * down) : (size.Height);
                bigmain.Margin = new Thickness((size.Width * down - bigmain.Width) / 2,
                    (size.Height - bigmain.Height) / 2,
                    (size.Width * down - bigmain.Width) / 2,
                    (size.Height - bigmain.Height) / 2);
            }
            headergrid.Width = headerRight.Width > headerRight.Height ? headerRight.Height : headerRight.Width;
            headergrid.Height = headerRight.Width > headerRight.Height ? headerRight.Height : headerRight.Width;
            headergrid.Margin = new Thickness((headerRight.Width - headergrid.Width) / 2,
                (headerRight.Height - headergrid.Height) / 2,
                (headerRight.Width - headergrid.Width) / 2,
                (headerRight.Height - headergrid.Height) / 2);
            //if (tiles != null)
               // foreach (Tile tile in tiles) { tile.SetCenterXY(b00.ActualWidth / 2, b00.ActualHeight / 2); }
        }

        private void menugrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            menutxt.Foreground = new SolidColorBrush(Colors.White);

            //设置鼠标手型
            //http://www.cnblogs.com/webabcd/archive/2013/01/10/2853974.html
            Windows.UI.Core.CoreWindow.GetForCurrentThread().PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 1);
            menugrid.PointerExited += (o, e1) =>
            {
                try { menutxt.Foreground = Application.Current.Resources["txt"] as SolidColorBrush; }
                catch (Exception) { menutxt.Foreground = new SolidColorBrush(Colors.Black); }
                CoreWindow.GetForCurrentThread().PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
            };
        }
        private void newgrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            newtxt.Foreground = new SolidColorBrush(Colors.White);
            CoreWindow.GetForCurrentThread().PointerCursor = new CoreCursor(CoreCursorType.Hand, 1);
            newgrid.PointerExited += (o, e1) =>
            {
                try { newtxt.Foreground = Application.Current.Resources["txt"] as SolidColorBrush; }
                catch (Exception) { newtxt.Foreground = new SolidColorBrush(Colors.Black); }
                CoreWindow.GetForCurrentThread().PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
            };
        }

        private void menugrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //UWP 系统级页面导航
            //http://www.cnblogs.com/Zixx/archive/2015/10/11/4868937.html
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(Menu));
        }

        private void newgrid_Tapped(object sender, TappedRoutedEventArgs e) { NewGame(nth++); }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //使得键盘焦点总不消失
            btn.Focus(FocusState.Programmatic);
            btn.LostFocus += Btn_LostFocus;
        }

        private void Btn_LostFocus(object sender, RoutedEventArgs e)
        {
            btn.Focus(FocusState.Programmatic);
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            tiles = new Tile[4, 4];
            tiles[0, 0] = b00;
            tiles[0, 1] = b01;
            tiles[0, 2] = b02;
            tiles[0, 3] = b03;
            tiles[1, 0] = b10;
            tiles[1, 1] = b11;
            tiles[1, 2] = b12;
            tiles[1, 3] = b13;
            tiles[2, 0] = b20;
            tiles[2, 1] = b21;
            tiles[2, 2] = b22;
            tiles[2, 3] = b23;
            tiles[3, 0] = b30;
            tiles[3, 1] = b31;
            tiles[3, 2] = b32;
            tiles[3, 3] = b33;
            NewGame(nth++);
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    App.UserData.CurrentInstance.Num[i, j] = tiles[i, j].Number;
            base.OnNavigatedFrom(e);
        }
    }

}

/*
     private void Move(int o)
     {
         // 初始化num中间数组为全零
         num = new int[4, 4];
         for (int i = 0; i < 4; i++)
         {
             for (int j = 0; j < 4; j++)
             {
                 num[i, j] = 0;
             }
         }

         //清除没有数字的格子
         bool hasBlankMove = ClearBlank(o);

         /** 记录得分 *
         int i_score = int.Parse(score.Text);

         bool hasAddMove = AddNumber(o, ref i_score);

         score.Text = i_score.ToString();
         if (i_score > int.Parse(best.Text)) { best.Text = i_score.ToString(); }

         /** 产生新的数字块 *
         if (hasAddMove | hasBlankMove)
         {
             // 产生1个新的数字
             Random random = new Random();
             int a = random.Next(15);
             if (a == 0)
                 a = 4;
             else
                 a = 2;
             int x = 0, y = 0;
             do
             {   // 产生[0,3]随机数
                 x = random.Next(4);
                 y = random.Next(4);
             } while (tiles[x, y].Number != 0);
             tiles[x, y].Number = a;
             //新数字出现动画
             tiles[x, y].Appera();
         }

         for (int i = 0; i < 4; i++)
         {
             for (int j = 0; j < 4; j++)
             {
                 Debug.Write(tiles[i, j].Number + " ");

                 //更新待保存数据
                 UserData.CurrentInstance.Num[i, j] = tiles[i, j].Number;

             }
             Debug.WriteLine("");
         }
         Debug.WriteLine("score = " + score.Text + " best = " + best.Text);
         UserData.CurrentInstance.Score = score.Text;
         UserData.CurrentInstance.Best = best.Text;
     }
     */
#region
/*
private bool ClearBlank(int o)
{
    bool hasBlankMove = false;
    if (o == LEFT)
    {
        for (int i = 0; i < 4; i++)
        {
            int t = 0;
            for (int j = 0; j < 4; j++)
            {
                if (0 != tiles[i, j].Number)
                {
                    num[i, t++] = tiles[i, j].Number;
                }
            }
        }
    }//LEFT

    if (o == RIGHT)
    {
        for (int i = 0; i < 4; i++)
        {
            int t = 3;
            for (int j = 3; j >= 0; j--)
            {
                if (0 != tiles[i, j].Number)
                {
                    num[i, t--] = tiles[i, j].Number;
                }
            }
        }
    }// RIGHT

    if (o == UP)
    {
        for (int j = 0; j < 4; j++)
        {
            int t = 0;
            for (int i = 0; i < 4; i++)
            {
                if (0 != tiles[i, j].Number)
                {
                    num[t++, j] = tiles[i, j].Number;
                }
            }
        }
    }// UP

    if (o == DOWN)
    {
        for (int j = 0; j < 4; j++)
        {
            int t = 3;
            for (int i = 3; i >= 0; i--)
            {
                if (0 != tiles[i, j].Number)
                {
                    // hasmove = true;
                    num[t--, j] = tiles[i, j].Number;
                }
            }
        }
    }//DOWN

    // 更新numbers
    for (int i = 0; i < 4; i++)
    {
        for (int j = 0; j < 4; j++)
        {
            // 移除空白后与原来不相同,说明有移动
            if (tiles[i, j].Number != num[i, j])
                hasBlankMove = true;
            tiles[i, j].Number = num[i, j];
        }
    }

    return hasBlankMove;
}

/// <summary>
/// 合并数字并更新得分
/// </summary>
/// <param name="o">方向</param>
/// <param name="s">得分</param>
/// <returns></returns>
private bool AddNumber(int o, ref int s)
{

    bool hasAddMove = false;
    if (o == LEFT)
    {
        Debug.WriteLine("←");
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (tiles[i, j].Number == tiles[i, j + 1].Number
                        && tiles[i, j].Number != 0)
                {
                    //放大的动画
                    tiles[i, j].Zoom();

                    tiles[i, j].Number += tiles[i, j + 1].Number;
                    s += tiles[i, j].Number;
                    hasAddMove = true;
                    for (int t = j + 1; t < 3; t++)
                    {
                        tiles[i, t].Number = tiles[i, t + 1].Number;
                    }
                    tiles[i, 3].Number = 0;
                }
            }
        }// 每行

    }// LEFT

    if (o == RIGHT)
    {
        Debug.WriteLine("→");
        for (int i = 0; i < 4; i++)
        {
            for (int j = 3; j > 0; j--)
            {
                if (tiles[i, j].Number == tiles[i, j - 1].Number
                        && tiles[i, j].Number != 0)
                {
                    //放大的动画
                    tiles[i, j].Zoom();

                    tiles[i, j].Number += tiles[i, j - 1].Number;
                    s += tiles[i, j].Number;
                    hasAddMove = true;
                    for (int t = j - 1; t > 0; t--)
                    {
                        tiles[i, t].Number = tiles[i, t - 1].Number;
                    }
                    tiles[i, 0].Number = 0;
                }
            }
        }
    }// RIGHT

    if (o == UP)
    {
        Debug.WriteLine("↑");
        for (int j = 0; j < 4; j++)
        {
            for (int i = 0; i < 3; i++)
            {
                if (tiles[i, j].Number == tiles[i + 1, j].Number
                        && tiles[i, j].Number != 0)
                {
                    //放大的动画
                    tiles[i, j].Zoom();

                    tiles[i, j].Number += tiles[i + 1, j].Number;
                    hasAddMove = true;
                    s += tiles[i, j].Number;
                    for (int t = i + 1; t < 3; t++)
                    {
                        tiles[t, j].Number = tiles[t + 1, j].Number;
                    }
                    tiles[3, j].Number = 0;
                }
            }
        }
    }// UP

    if (o == DOWN)
    {
        Debug.WriteLine("↓");
        for (int j = 0; j < 4; j++)
        {
            for (int i = 3; i > 0; i--)
            {
                if (tiles[i, j].Number == tiles[i - 1, j].Number
                        && tiles[i, j].Number != 0)
                {
                    //放大的动画
                    tiles[i, j].Zoom();

                    tiles[i, j].Number += tiles[i - 1, j].Number;
                    hasAddMove = true;
                    s += tiles[i, j].Number;
                    for (int t = i - 1; t > 0; t--)
                    {
                        tiles[t, j].Number = tiles[t - 1, j].Number;
                    }
                    tiles[0, j].Number = 0;
                }
            }
        }
    }//DOWN

    return hasAddMove;
}

private bool IsGameOver()
{
    for (int i = 0; i < 3; i++)
    {
        for (int j = 0; j < 3; j++)
        {
            if (tiles[i, j].Number == 0)
            {
                return false;// 还有空位置
            }
            else
            {
                if (tiles[i, j].Number == tiles[i, j + 1].Number
                        || tiles[i, j].Number == tiles[i + 1, j].Number)
                {
                    return false;// 有相等的块
                }
            }
        }
        /**
         * 外层大for循环只判断了3*3格，这里判断最外几个数字(下面x处)
         * 
         * <pre>
         * · · · x
         * · · · x
         * · · · x
         * x x x x
         * </pre>
         *
        if (tiles[3, i].Number == tiles[3, i + 1].Number
                || tiles[i, 3].Number == tiles[i + 1, 3].Number
                || tiles[3, i].Number == 0 || tiles[i, 3].Number == 0)
        {
            return false;// 有相等的块或边上有空白块
        }
    }
    return true; // 没有了空位置了,也没有相邻相等的数字
}

private async void IfGameOver()
{
    if (IsGameOver())
    {

        ContentDialog dialog = new ContentDialog()
        {
            Title = "游戏结束", //标题
            Content = "\n已无法合并任何数字\n您的得分为:" + score.Text + ",最高分为:" + best.Text + ".",//内容
            FullSizeDesired = false,  //是否全屏展示
            PrimaryButtonText = "新游戏",//第一个按钮内容
            SecondaryButtonText = "返回"
        };
        try
        {
            var result = await dialog.ShowAsync();
            if (ContentDialogResult.Primary == result)
            {
                NewGame(times++);
            }
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
    }
}

private void NewGame_Click(object sender, RoutedEventArgs e)
{
    NewGame(times++);
}

private void Init()
{
    //重置为零
    foreach (Tile t in tiles)
    {
        t.Number = 0;
    }
    Random random = new Random();
    int a = 4;
    if (random.Next(0, 10) != 0) a = 2;
    int x1 = random.Next(0, 4),
        y1 = random.Next(0, 4);
    int x2, y2;
    do
    {
        x2 = random.Next(0, 4);
        y2 = random.Next(0, 4);
    } while (x1 == x2 && y1 == y2);
    //其中一个为2，另一个为2(90%)或4(10%)
    tiles[x1, y1].Number = 2;
    tiles[x2, y2].Number = a;
    score.Text = "0";
}

private void NewGame(int nth)
{
    //刚打开游戏，需要读取上次游戏(如果有)
    if (nth == 0)
    {
        score.Text = UserData.CurrentInstance.Score;
        best.Text = UserData.CurrentInstance.Best;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                tiles[i, j].Number = UserData.CurrentInstance.Num[i, j];
            }
        }
        if (best.Text.Equals("0")) { Init(); }
    }
    else Init();
}
*/
#endregion

#region
/*
#region
protected override void OnPointerPressed(PointerRoutedEventArgs e)
{
    base.OnPointerPressed(e);
    PointerPoint point = e.GetCurrentPoint(null);
    _startPoint = point;
    _recognizer.ProcessDownEvent(point);
    _recognizer1.ProcessDownEvent(point);
    Debug.WriteLine("Pressed");
}

protected override void OnPointerMoved(PointerRoutedEventArgs e)
{
    base.OnPointerMoved(e);
    _recognizer.ProcessMoveEvents(e.GetIntermediatePoints(null));
    _recognizer1.ProcessMoveEvents(e.GetIntermediatePoints(null));
}

protected override void OnPointerReleased(PointerRoutedEventArgs e)
{
    base.OnPointerReleased(e);
    PointerPoint point = e.GetCurrentPoint(null);
    _endPoint = point;
    _recognizer.ProcessUpEvent(point);
    _recognizer1.ProcessUpEvent(point);
}
private void GestureRecognizer_CrossSliding1(GestureRecognizer sender, CrossSlidingEventArgs args)
{
    if (args.CrossSlidingState == CrossSlidingState.Completed
        //&& (args.PointerDeviceType == PointerDeviceType.Pen || args.PointerDeviceType == PointerDeviceType.Touch)
        )// 鼠标就不处理了。
    {
        if (_startPoint != null && _endPoint != null)// 确保访问 Position 不会异常。
        {
            double startX = _startPoint.Position.X;
            double endX = _endPoint.Position.X;
            if (startX < endX)
            {
                Debug.WriteLine("上");
            }
            else if (startX > endX)
            {
                Debug.WriteLine("下");
            }
        }
    }
}

private void GestureRecognizer_CrossSliding(GestureRecognizer sender, CrossSlidingEventArgs args)
{
    if (args.CrossSlidingState == CrossSlidingState.Completed
        && (args.PointerDeviceType == PointerDeviceType.Pen || args.PointerDeviceType == PointerDeviceType.Touch)
        )// 鼠标就不处理了。
    {
        if (_startPoint != null && _endPoint != null)// 确保访问 Position 不会异常。
        {
            double startX = _startPoint.Position.X;
            double endX = _endPoint.Position.X;
            if (startX < endX)
            {
                Debug.WriteLine("右");
            }
            else if (startX > endX)
            {
                Debug.WriteLine("左");
            }
        }
    }
}
#endregion
*/
#endregion

/*        private readonly GestureRecognizer _recognizer;
        private PointerPoint _startPoint;
        private PointerPoint _endPoint;

        private readonly GestureRecognizer _recognizer1;
        _recognizer = new GestureRecognizer()
        {
            // 识别滑动手势。
            GestureSettings = GestureSettings.CrossSlide,
            // 识别水平滑动手势。
            CrossSlideHorizontally = true
        };
        _recognizer.CrossSliding += GestureRecognizer_CrossSliding;

        _recognizer1 = new GestureRecognizer()
        {
            // 识别滑动手势。
            GestureSettings = GestureSettings.CrossSlide,
            // 识别垂直滑动手势。
            CrossSlideHorizontally = false
        };
        _recognizer1.CrossSliding += GestureRecognizer_CrossSliding1;
        */
