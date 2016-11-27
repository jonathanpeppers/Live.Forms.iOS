using System;
using System.Linq;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Live.Forms.iOS
{
    internal static class LiveXaml
    {
        private static List<XamlTimer> _timers = new List<XamlTimer>();

        public static void Watch(Element view)
        {
            string file;
            if (!string.IsNullOrEmpty(view.ClassId))
            {
                file = view.ClassId.Split('.').Last() + ".xaml";
            }
            else
            {
                file = view.GetType().Name + ".xaml";
            }
            
            Watch(view, file);
        }

        private static void Watch(Element view, string file)
        {
            try
            {
                string directory = Configuration.Directory;
                if (string.IsNullOrEmpty(directory))
                {
                    throw new ArgumentNullException("Configuration.Directory");
                }

                string path = Directory.GetFiles(directory, file, SearchOption.AllDirectories).FirstOrDefault();
                if (string.IsNullOrEmpty(path))
                {
                    throw new Exception("Could not find XAML file named " + file + "!");
                }

                var timer = _timers.FirstOrDefault(t => t.Path == path);
                if (timer == null)
                {
                    timer = new XamlTimer(path, view);
                    timer.FileChanged += OnFileChanged;
                    _timers.Add(timer);
                }
                else
                {
                    lock (timer)
                    {
                        timer.Views.Add(new WeakReference(view));
                        if (timer.HasEverChanged)
                        {
                            UpdateViews(timer, false, view);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                OnError(exc);
            }
        }

        private static void OnError(Exception exc)
        {
            //TODO: on error?
            //Console.WriteLine("Error in LiveXamlLoader: " + exc);
        }

        private static void OnFileChanged(object sender, EventArgs e)
        {
            try
            {
                var timer = (XamlTimer)sender;
                lock (timer)
                {
                    var views = new List<Element>();
                    foreach (var weakReference in timer.Views.ToArray())
                    {
                        var view = weakReference.Target as Element;
                        if (view != null)
                        {
                            views.Add(view);
                        }
                        else
                        {
                            timer.Views.Remove(weakReference);
                        }
                    }

                    if (views.Count > 0)
                        UpdateViews(timer, true, views.ToArray());
                }
            }
            catch (Exception exc)
            {
                OnError(exc);
            }
        }

        private static void UpdateViews(XamlTimer timer, bool fromTimer, params Element[] views)
        {
            string xaml = File.ReadAllText(timer.Path);
            foreach (var view in views)
            {
                if (fromTimer && view.Parent == null)
                {
                    //We will let WeakReference clean this up
                    continue;
                }
                else
                {
                    //NOTE: for any XAML that sets lists, we have to clear the list for things to work right
                    var grid = view as Grid;
                    if (grid != null)
                    {
                        grid.Children.Clear();
                        grid.RowDefinitions.Clear();
                        grid.ColumnDefinitions.Clear();
                    }
                    else
                    {
                        var layout = view as Layout<View>;
                        if (layout != null)
                        {
                            layout.Children.Clear();
                        }
                    }

                    XamlLoader.Load(view, xaml);
                    UpdateNames(view);
                }
            }
        }

        private static void UpdateNames(Element view)
        {
            var type = view.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(m =>
                {
                    var attribute = m.GetCustomAttribute<GeneratedCodeAttribute>();
                    return attribute != null && attribute.Tool == "Xamarin.Forms.Build.Tasks.XamlG"; //Xamarin.Forms, you tool!
                });
            foreach (var field in fields)
            {
                field.SetValue(view, view.FindByName<object>(field.Name));
            }
        }

        private class XamlTimer : Timer
        {
            private DateTime _lastTime;

            public string Path { get; private set; }

            public Type Type { get; private set; }

            public List<WeakReference> Views { get; private set; }

            public bool HasEverChanged { get; private set; }

            public event EventHandler FileChanged;

            public XamlTimer(string path, Element view) : base(1000)
            {
                Path = path;
                Type = view.GetType();
                Views = new List<WeakReference> { new WeakReference(view) };

                _lastTime = new FileInfo(Path).LastWriteTimeUtc;
                Elapsed += OnElapsed;
                Start();
            }

            private void OnElapsed(object sender, EventArgs e)
            {
                DateTime time = new FileInfo(Path).LastWriteTimeUtc;
                if (_lastTime != time)
                {
                    _lastTime = time;
                    HasEverChanged = true;

                    var handler = FileChanged;
                    if (handler != null)
                        Device.BeginInvokeOnMainThread(() => handler(this, EventArgs.Empty));
                }
            }
        }
    }
}

