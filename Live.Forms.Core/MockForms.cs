﻿using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Live.Forms
{
    public static class MockForms
    {
        public static void Init()
        {
            Device.PlatformServices = new PlatformServices();
            DependencyService.Register<SystemResourcesProvider>();
        }

        private class TestTicker : Ticker
        {
            private bool _inside = false;

            public override int Insert(Func<long, bool> timeout)
            {
                var result = base.Insert(timeout);
                if (!_inside)
                {
                    _inside = true;
                    timeout(Environment.TickCount);
                    _inside = false;
                }
                return result;
            }

            protected override void DisableTimer() { }

            protected override void EnableTimer() { }
        }

        private class PlatformServices : IPlatformServices
        {
            public bool IsInvokeRequired
            {
                get { return false; }
            }

            public void BeginInvokeOnMainThread(Action action)
            {
                action();
            }

            public Ticker CreateTicker()
            {
                return new TestTicker();
            }

            public ITimer CreateTimer(Action<object> callback)
            {
                throw new NotImplementedException();
            }

            public ITimer CreateTimer(Action<object> callback, object state, uint dueTime, uint period)
            {
                throw new NotImplementedException();
            }

            public ITimer CreateTimer(Action<object> callback, object state, TimeSpan dueTime, TimeSpan period)
            {
                throw new NotImplementedException();
            }

            public ITimer CreateTimer(Action<object> callback, object state, long dueTime, long period)
            {
                throw new NotImplementedException();
            }

            public ITimer CreateTimer(Action<object> callback, object state, int dueTime, int period)
            {
                throw new NotImplementedException();
            }

            public Assembly[] GetAssemblies()
            {
                return new Assembly[0];
            }

            public string GetMD5Hash(string input)
            {
                throw new NotImplementedException();
            }

            public double GetNamedSize(NamedSize size, Type targetElementType, bool useOldSizes)
            {
                return 14;
            }

            public Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public IIsolatedStorageFile GetUserStoreForApplication()
            {
                throw new NotImplementedException();
            }

            public void OpenUriAction(Uri uri) { }

            public void StartTimer(TimeSpan interval, Func<bool> callback) { }
        }

        private class SystemResourcesProvider : ISystemResourcesProvider
        {
            private ResourceDictionary _dictionary = new ResourceDictionary();

            public IResourceDictionary GetSystemResources()
            {
                return _dictionary;
            }
        }
    }
}
