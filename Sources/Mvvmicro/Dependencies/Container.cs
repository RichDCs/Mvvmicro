﻿namespace Mvvmicro
{
    using System;
    using System.Collections.Generic;

    public class Container : IContainer
    {
        #region Default

        private static Lazy<IContainer> instance = new Lazy<IContainer>(() => new Container());

        /// <summary>
        /// Gets the default container.
        /// </summary>
        /// <value>The default.</value>
        public static IContainer Default => instance.Value;

        #endregion

        #region Fields

        private Dictionary<Type, object> instances = new Dictionary<Type, object>();

        private Dictionary<Type, Tuple<bool, Func<object>>> factories = new Dictionary<Type, Tuple<bool, Func<object>>>();

        #endregion

        #region Methods

        public T Get<T>()
        {
            var factory = this.factories[typeof(T)];

            if (factory.Item1)
            {
                if (instances.TryGetValue(typeof(T), out object instance))
                {
                    return (T)instance;
                }

                var newInstance = factory.Item2();
                instances[typeof(T)] = newInstance;
                return (T)newInstance;
            }

            return (T)factory.Item2();
        }

        public void Register<T>(Func<IContainer, T> factory, bool isInstance = false)
        {
            this.factories[typeof(T)] = new Tuple<bool, Func<object>>(isInstance, () => factory(this));
        }

        public bool IsRegistered<T>() => factories.ContainsKey(typeof(T));

        public void Unregister<T>()
        {
            if (this.IsRegistered<T>())
            {
                instances.Remove(typeof(T));
                factories.Remove(typeof(T));
            }
        }

        public void WipeContainer()
        {
            this.instances = new Dictionary<Type, object>();
            this.factories = new Dictionary<Type, Tuple<bool, Func<object>>>();
        }

        public T New<T>() => (T)this.factories[typeof(T)].Item2();

        #endregion
    }
}
