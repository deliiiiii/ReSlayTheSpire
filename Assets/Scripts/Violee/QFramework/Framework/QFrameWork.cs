﻿/****************************************************************************
 * Copyright (c) 2015 ~ 2024 liangxiegame MIT License
 *
 * QFramework v1.0
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 *
 * Author:
 *  liangxie        https://github.com/liangxie
 *  soso            https://github.com/so-sos-so
 *
 * Contributor
 *  TastSong        https://github.com/TastSong
 *  京产肠饭         https://gitee.com/JingChanChangFan/hk_-unity-tools
 *  猫叔(一只皮皮虾) https://space.bilibili.com/656352/
 *  misakiMeiii     https://github.com/misakiMeiii
 *  New一天
 *  幽飞冷凝雪～冷
 *
 * Community
 *  QQ Group: 623597263
 * 
 * Latest Update: 2025.3.18 10:21 add InitArchitecture api
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
#pragma warning disable CS8602 // 解引用可能出现空引用。
#pragma warning disable CS8604 // 引用类型参数可能为 null。
#pragma warning disable CS8601 // 引用类型赋值可能为 null。
#pragma warning disable CS8625 // 无法将 null 字面量转换为非 null 的引用类型。
#pragma warning disable CS8603 // 可能返回 null 引用。
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 'required' 修饰符或声明为可以为 null。

namespace QFramework
{
    #region Architecture

    public interface IArchitecture
    {
        void RegisterSystem<T>(T system) where T : ISystem;

        void RegisterModel<T>(T model) where T : IModel;

        void RegisterUtility<T>(T utility) where T : IUtility;

        T GetSystem<T>() where T : class, ISystem;

        T GetModel<T>() where T : class, IModel;

        T GetUtility<T>() where T : class, IUtility;

        void SendCommand<T>(T command) where T : ICommand;

        TResult SendCommand<TResult>(ICommand<TResult> command);

        TResult SendQuery<TResult>(IQuery<TResult> query);

        void SendEvent<T>() where T : new();
        void SendEvent<T>(T e);

        IUnRegister RegisterEvent<T>(Action<T> onEvent);
        void UnRegisterEvent<T>(Action<T> onEvent);

        void Deinit();
    }

    public abstract class Architecture<T> : IArchitecture where T : Architecture<T>, new()
    {
        bool mInited;


        public static readonly Action<T> OnRegisterPatch = _ => { };

        protected static T mArchitecture;

        public static IArchitecture Interface
        {
            get
            {
                if (mArchitecture == null) InitArchitecture();
                return mArchitecture;
            }
        }


        public static void InitArchitecture()
        {
            if (mArchitecture == null)
            {
                mArchitecture = new T();
                mArchitecture.Init();

                OnRegisterPatch?.Invoke(mArchitecture);

                foreach (var model in mArchitecture.mContainer.GetInstancesByType<IModel>().Where(m => !m.Initialized))
                {
                    model.Init();
                    model.Initialized = true;
                }

                foreach (var system in mArchitecture.mContainer.GetInstancesByType<ISystem>()
                             .Where(m => !m.Initialized))
                {
                    system.Init();
                    system.Initialized = true;
                }

                mArchitecture.mInited = true;
            }
        }

        protected abstract void Init();

        public void Deinit()
        {
            OnDeinit();
            foreach (var system in mContainer.GetInstancesByType<ISystem>().Where(s => s.Initialized)) system.Deinit();
            foreach (var model in mContainer.GetInstancesByType<IModel>().Where(m => m.Initialized)) model.Deinit();
            mContainer.Clear();
            mArchitecture = null;
        }

        protected virtual void OnDeinit()
        {
        }

        private IocContainer mContainer = new IocContainer();

        public void RegisterSystem<TSystem>(TSystem system) where TSystem : ISystem
        {
            system.SetArchitecture(this);
            mContainer.Register(system);

            if (mInited)
            {
                system.Init();
                system.Initialized = true;
            }
        }

        public void RegisterModel<TModel>(TModel model) where TModel : IModel
        {
            model.SetArchitecture(this);
            mContainer.Register(model);

            if (mInited)
            {
                model.Init();
                model.Initialized = true;
            }
        }

        public void RegisterUtility<TUtility>(TUtility utility) where TUtility : IUtility =>
            mContainer.Register(utility);

        public TSystem GetSystem<TSystem>() where TSystem : class, ISystem => mContainer.Get<TSystem>();

        public TModel GetModel<TModel>() where TModel : class, IModel => mContainer.Get<TModel>();

        public TUtility GetUtility<TUtility>() where TUtility : class, IUtility => mContainer.Get<TUtility>();

        public TResult SendCommand<TResult>(ICommand<TResult> command) => ExecuteCommand(command);

        public void SendCommand<TCommand>(TCommand command) where TCommand : ICommand => ExecuteCommand(command);

        protected virtual TResult ExecuteCommand<TResult>(ICommand<TResult> command)
        {
            command.SetArchitecture(this);
            return command.Execute();
        }

        protected virtual void ExecuteCommand(ICommand command)
        {
            command.SetArchitecture(this);
            command.Execute();
        }

        public TResult SendQuery<TResult>(IQuery<TResult> query) => DoQuery(query);

        protected virtual TResult DoQuery<TResult>(IQuery<TResult> query)
        {
            query.SetArchitecture(this);
            return query.Do();
        }

        readonly TypeEventSystem mTypeEventSystem = new();

        public void SendEvent<TEvent>() where TEvent : new() => mTypeEventSystem.Send<TEvent>();

        public void SendEvent<TEvent>(TEvent e) => mTypeEventSystem.Send(e);

        public IUnRegister RegisterEvent<TEvent>(Action<TEvent> onEvent) => mTypeEventSystem.Register(onEvent);

        public void UnRegisterEvent<TEvent>(Action<TEvent> onEvent) => mTypeEventSystem.UnRegister(onEvent);
    }

    public interface IOnEvent<T>
    {
        void OnEvent(T e);
    }

    public static class OnGlobalEventExtension
    {
        public static IUnRegister RegisterEvent<T>(this IOnEvent<T> self) where T : struct =>
            TypeEventSystem.Global.Register<T>(self.OnEvent);

        public static void UnRegisterEvent<T>(this IOnEvent<T> self) where T : struct =>
            TypeEventSystem.Global.UnRegister<T>(self.OnEvent);
    }

    #endregion

    #region Controller

    public interface IController : ICanSendCommand, ICanGetSystem, ICanGetModel,
        ICanRegisterEvent, ICanSendQuery, ICanGetUtility
    {
    }

    #endregion

    #region System

    public interface ISystem : ICanSetArchitecture, ICanGetModel, ICanGetUtility,
        ICanRegisterEvent, ICanSendEvent, ICanGetSystem, ICanInit
    {
    }

    public abstract class AbstractSystem : ISystem
    {
        public IArchitecture mArchitecture { get; set; }

        IArchitecture IBelongToArchitecture.GetArchitecture() => mArchitecture;

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => mArchitecture = architecture;

        public bool Initialized { get; set; }
        void ICanInit.Init() => OnInit();

        public void Deinit() => OnDeinit();

        protected virtual void OnDeinit()
        {
        }

        protected abstract void OnInit();
    }

    #endregion

    #region Model

    public interface IModel : ICanSetArchitecture, ICanGetUtility, ICanSendEvent, ICanInit
    {
    }

    public abstract partial class AbstractModel : IModel
    {
        public IArchitecture mArchitecture { get; set; }
        public bool Initialized { get; set; }
        void ICanInit.Init() => OnInit();
        public void Deinit() => OnDeinit();

        protected virtual void OnDeinit()
        {
        }

        protected abstract void OnInit();
    }

    #endregion

    #region Utility

    public interface IUtility
    {
    }

    #endregion

    #region Command

    public interface ICommand : ICanSetArchitecture, ICanGetSystem, ICanGetModel, ICanGetUtility,
        ICanSendEvent, ICanSendCommand, ICanSendQuery
    {
        void Execute();
    }

    public interface ICommand<TResult> : ICanSetArchitecture, ICanGetSystem, ICanGetModel,
        ICanGetUtility,
        ICanSendEvent, ICanSendCommand, ICanSendQuery
    {
        TResult Execute();
    }

    public abstract class AbstractCommand : ICommand
    {
        // private IArchitecture mArchitecture;

        public IArchitecture mArchitecture { get; set; }
        IArchitecture IBelongToArchitecture.GetArchitecture() => mArchitecture;

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => mArchitecture = architecture;

        void ICommand.Execute() => OnExecute();

        protected abstract void OnExecute();
    }

    public abstract class AbstractCommand<TResult> : ICommand<TResult>
    {
        public IArchitecture mArchitecture { get; set; }
        TResult ICommand<TResult>.Execute() => OnExecute();

        protected abstract TResult OnExecute();
    }

    #endregion

    #region Query

    public interface IQuery<TResult> : ICanSetArchitecture, ICanGetModel, ICanGetSystem,
        ICanSendQuery
    {
        TResult Do();
    }

    public abstract class AbstractQuery<T> : IQuery<T>
    {
        public T Do() => OnDo();
        protected abstract T OnDo();
        public IArchitecture mArchitecture { get; set; }
    }

    #endregion

    #region Rule

    public interface IBelongToArchitecture
    {
        public IArchitecture mArchitecture { get; }
        IArchitecture GetArchitecture() => mArchitecture;
    }

    public interface ICanSetArchitecture
    {
        public IArchitecture mArchitecture { set; }
        void SetArchitecture(IArchitecture architecture) => mArchitecture = architecture;
    }

    public interface ICanGetModel : IBelongToArchitecture
    {
    }

    public static class CanGetModelExtension
    {
        public static T GetModel<T>(this ICanGetModel self) where T : class, IModel =>
            self.GetArchitecture().GetModel<T>();
    }

    public interface ICanGetSystem : IBelongToArchitecture
    {
    }

    public static class CanGetSystemExtension
    {
        public static T GetSystem<T>(this ICanGetSystem self) where T : class, ISystem =>
            self.GetArchitecture().GetSystem<T>();
    }

    public interface ICanGetUtility : IBelongToArchitecture
    {
    }

    public static class CanGetUtilityExtension
    {
        public static T GetUtility<T>(this ICanGetUtility self) where T : class, IUtility =>
            self.GetArchitecture().GetUtility<T>();
    }

    public interface ICanRegisterEvent : IBelongToArchitecture
    {
    }

    public static class CanRegisterEventExtension
    {
        public static IUnRegister RegisterEvent<T>(this ICanRegisterEvent self, Action<T> onEvent) =>
            self.GetArchitecture().RegisterEvent(onEvent);

        public static void UnRegisterEvent<T>(this ICanRegisterEvent self, Action<T> onEvent) =>
            self.GetArchitecture().UnRegisterEvent(onEvent);
    }

    public interface ICanSendCommand : IBelongToArchitecture
    {
    }

    public static class CanSendCommandExtension
    {
        public static void SendCommand<T>(this ICanSendCommand self) where T : ICommand, new() =>
            self.GetArchitecture().SendCommand(new T());

        public static void SendCommand<T>(this ICanSendCommand self, T command) where T : ICommand =>
            self.GetArchitecture().SendCommand(command);

        public static TResult SendCommand<TResult>(this ICanSendCommand self, ICommand<TResult> command) =>
            self.GetArchitecture().SendCommand(command);
    }

    public interface ICanSendEvent : IBelongToArchitecture
    {
    }

    public static class CanSendEventExtension
    {
        public static void SendEvent<T>(this ICanSendEvent self) where T : new() =>
            self.GetArchitecture().SendEvent<T>();

        public static void SendEvent<T>(this ICanSendEvent self, T e) => self.GetArchitecture().SendEvent(e);
    }

    public interface ICanSendQuery : IBelongToArchitecture
    {
    }

    public static class CanSendQueryExtension
    {
        public static TResult SendQuery<TResult>(this ICanSendQuery self, IQuery<TResult> query) =>
            self.GetArchitecture().SendQuery(query);
    }

    public interface ICanInit
    {
        bool Initialized { get; set; }
        void Init();
        void Deinit();
    }

    #endregion

    #region TypeEventSystem

    public interface IUnRegister
    {
        void UnRegister();
    }

    public interface IUnRegisterList
    {
        List<IUnRegister> UnregisterList { get; }
    }

    public static class IUnRegisterListExtension
    {
        public static void AddToUnregisterList(this IUnRegister self, IUnRegisterList unRegisterList) =>
            unRegisterList.UnregisterList.Add(self);

        public static void UnRegisterAll(this IUnRegisterList self)
        {
            foreach (var unRegister in self.UnregisterList)
            {
                unRegister.UnRegister();
            }

            self.UnregisterList.Clear();
        }
    }

    public struct CustomUnRegister : IUnRegister
    {
        private Action mOnUnRegister { get; set; }
        public CustomUnRegister(Action onUnRegister) => mOnUnRegister = onUnRegister;

        public void UnRegister()
        {
            mOnUnRegister.Invoke();
            mOnUnRegister = null;
        }
    }

#if UNITY_5_6_OR_NEWER
    public abstract class UnRegisterTrigger : MonoBehaviour
    {
        private readonly HashSet<IUnRegister> mUnRegisters = new HashSet<IUnRegister>();

        public IUnRegister AddUnRegister(IUnRegister unRegister)
        {
            mUnRegisters.Add(unRegister);
            return unRegister;
        }

        public void RemoveUnRegister(IUnRegister unRegister) => mUnRegisters.Remove(unRegister);

        public void UnRegister()
        {
            foreach (var unRegister in mUnRegisters)
            {
                unRegister.UnRegister();
            }

            mUnRegisters.Clear();
        }
    }

    public class UnRegisterOnDestroyTrigger : UnRegisterTrigger
    {
        private void OnDestroy()
        {
            UnRegister();
        }
    }

    public class UnRegisterOnDisableTrigger : UnRegisterTrigger
    {
        private void OnDisable()
        {
            UnRegister();
        }
    }

    public class UnRegisterCurrentSceneUnloadedTrigger : UnRegisterTrigger
    {
        private static UnRegisterCurrentSceneUnloadedTrigger mDefault;

        public static UnRegisterCurrentSceneUnloadedTrigger Get
        {
            get
            {
                if (!mDefault)
                {
                    mDefault = new GameObject("UnRegisterCurrentSceneUnloadedTrigger")
                        .AddComponent<UnRegisterCurrentSceneUnloadedTrigger>();
                }

                return mDefault;
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);
            hideFlags = HideFlags.HideInHierarchy;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDestroy() => SceneManager.sceneUnloaded -= OnSceneUnloaded;
        void OnSceneUnloaded(Scene scene) => UnRegister();
    }
#endif

    public static class UnRegisterExtension
    {
#if UNITY_5_6_OR_NEWER

        static T GetOrAddComponent<T>(GameObject gameObject) where T : Component
        {
            var trigger = gameObject.GetComponent<T>();

            if (!trigger)
            {
                trigger = gameObject.AddComponent<T>();
            }

            return trigger;
        }

        public static IUnRegister UnRegisterWhenGameObjectDestroyed(this IUnRegister unRegister,
            GameObject gameObject) =>
            GetOrAddComponent<UnRegisterOnDestroyTrigger>(gameObject)
                .AddUnRegister(unRegister);

        public static IUnRegister UnRegisterWhenGameObjectDestroyed<T>(this IUnRegister self, T component)
            where T : Component =>
            self.UnRegisterWhenGameObjectDestroyed(component.gameObject);

        public static IUnRegister UnRegisterWhenDisabled<T>(this IUnRegister self, T component)
            where T : Component =>
            self.UnRegisterWhenDisabled(component.gameObject);

        public static IUnRegister UnRegisterWhenDisabled(this IUnRegister unRegister,
            GameObject gameObject) =>
            GetOrAddComponent<UnRegisterOnDisableTrigger>(gameObject)
                .AddUnRegister(unRegister);
        
        public static IUnRegister UnRegisterWhenCurrentSceneUnloaded(this IUnRegister self) =>
            UnRegisterCurrentSceneUnloadedTrigger.Get.AddUnRegister(self);
#endif


#if GODOT
		public static IUnRegister UnRegisterWhenNodeExitTree(this IUnRegister unRegister, Godot.Node node)
		{
			node.TreeExiting += unRegister.UnRegister;
			return unRegister;
		}
#endif
    }

    public class TypeEventSystem
    {
        private readonly EasyEvents mEvents = new EasyEvents();

        public static readonly TypeEventSystem Global = new TypeEventSystem();

        public void Send<T>() where T : new() => mEvents.GetEvent<EasyEvent<T>>()?.Trigger(new T());

        public void Send<T>(T e) => mEvents.GetEvent<EasyEvent<T>>()?.Trigger(e);

        public IUnRegister Register<T>(Action<T> onEvent) => mEvents.GetOrAddEvent<EasyEvent<T>>().Register(onEvent);

        public void UnRegister<T>(Action<T> onEvent)
        {
            var e = mEvents.GetEvent<EasyEvent<T>>();
            e?.UnRegister(onEvent);
        }
    }

    #endregion

    #region IOC

    public class IocContainer
    {
        readonly Dictionary<Type, object> mInstances = new();

        public void Register<T>(T instance)
        {
            var key = typeof(T);

            mInstances[key] = instance;
        }

        public T Get<T>() where T : class
        {
            var key = typeof(T);

            if (mInstances.TryGetValue(key, out var retInstance))
            {
                return retInstance as T;
            }

            return null;
        }

        public IEnumerable<T> GetInstancesByType<T>()
        {
            var type = typeof(T);
            return mInstances.Values.Where(instance => type.IsInstanceOfType(instance)).Cast<T>();
        }

        public void Clear() => mInstances.Clear();
    }

    #endregion

    #region BindableProperty

    public interface IBindableProperty<T> : IReadonlyBindableProperty<T>
    {
        new T Value { get; set; }
        void SetValueWithoutEvent(T newValue);
    }

    public interface IReadonlyBindableProperty<T> : IEasyEvent
    {
        T Value { get; }

        IUnRegister RegisterWithInitValue(Action<T> action);
        void UnRegister(Action<T> onValueChanged);
        IUnRegister Register(Action<T> onValueChanged);
    }

    public class BindableProperty<T> : IBindableProperty<T>
    {
        public BindableProperty(T defaultValue = default) => mValue = defaultValue;

        protected T mValue;

        public static Func<T, T, bool> Comparer { get; set; } = (a, b) => a.Equals(b);

        public BindableProperty<T> WithComparer(Func<T, T, bool> comparer)
        {
            Comparer = comparer;
            return this;
        }

        public T Value
        {
            get => GetValue();
            set
            {
                if (value == null && mValue == null) return;
                if (value != null && Comparer(value, mValue)) return;

                SetValue(value);
                mOnValueChanged.Trigger(value);
            }
        }

        protected virtual void SetValue(T newValue) => mValue = newValue;

        protected virtual T GetValue() => mValue;

        public void SetValueWithoutEvent(T newValue) => mValue = newValue;

        private EasyEvent<T> mOnValueChanged = new EasyEvent<T>();

        public IUnRegister Register(Action<T> onValueChanged)
        {
            return mOnValueChanged.Register(onValueChanged);
        }

        public IUnRegister RegisterWithInitValue(Action<T> onValueChanged)
        {
            onValueChanged(mValue);
            return Register(onValueChanged);
        }

        public void UnRegister(Action<T> onValueChanged) => mOnValueChanged.UnRegister(onValueChanged);

        IUnRegister IEasyEvent.Register(Action onEvent)
        {
            return Register(Action);
            void Action(T _) => onEvent();
        }

        public override string ToString() => Value.ToString();
    }

    internal class ComparerAutoRegister
    {
#if UNITY_5_6_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AutoRegister()
        {
            BindableProperty<int>.Comparer = (a, b) => a == b;
            BindableProperty<float>.Comparer = Mathf.Approximately;
            BindableProperty<double>.Comparer = (a, b) => Math.Abs(a - b) < 0.00001;
            BindableProperty<string>.Comparer = (a, b) => a == b;
            BindableProperty<long>.Comparer = (a, b) => a == b;
            BindableProperty<Vector2>.Comparer = (a, b) => a == b;
            BindableProperty<Vector3>.Comparer = (a, b) => a == b;
            BindableProperty<Vector4>.Comparer = (a, b) => a == b;
            BindableProperty<Color>.Comparer = (a, b) => a == b;
            BindableProperty<Color32>.Comparer =
                (a, b) => a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
            BindableProperty<Bounds>.Comparer = (a, b) => a == b;
            BindableProperty<Rect>.Comparer = (a, b) => a == b;
            BindableProperty<Quaternion>.Comparer = (a, b) => a == b;
            BindableProperty<Vector2Int>.Comparer = (a, b) => a == b;
            BindableProperty<Vector3Int>.Comparer = (a, b) => a == b;
            BindableProperty<BoundsInt>.Comparer = (a, b) => a == b;
            BindableProperty<RangeInt>.Comparer = (a, b) => a.start == b.start && a.length == b.length;
            BindableProperty<RectInt>.Comparer = (a, b) => a.Equals(b);
        }
#endif
    }

    #endregion

    #region EasyEvent

    public interface IEasyEvent
    {
        IUnRegister Register(Action onEvent); 
    }

    public class EasyEvent : IEasyEvent
    {
        private Action mOnEvent = () => { };

        public IUnRegister Register(Action onEvent)
        {
            mOnEvent += onEvent;
            return new CustomUnRegister(() => { UnRegister(onEvent); });
        }

        public IUnRegister RegisterWithACall(Action onEvent)
        {
            onEvent.Invoke();
            return Register(onEvent);
        }

        public void UnRegister(Action onEvent) => mOnEvent -= onEvent;

        public void Trigger() => mOnEvent?.Invoke();
    }

    public class EasyEvent<T> : IEasyEvent
    {
        private Action<T> mOnEvent = _ => { };

        public IUnRegister Register(Action<T> onEvent)
        {
            mOnEvent += onEvent;
            return new CustomUnRegister(() => { UnRegister(onEvent); });
        }

        public void UnRegister(Action<T> onEvent) => mOnEvent -= onEvent;
        

        public void Trigger(T t) => mOnEvent?.Invoke(t);

        IUnRegister IEasyEvent.Register(Action onEvent)
        {
            return Register(Action);
            void Action(T _) => onEvent();
        }
    }

    public class EasyEvent<T, TK> : IEasyEvent
    {
        private Action<T, TK> mOnEvent = (_, _) => { };

        public IUnRegister Register(Action<T, TK> onEvent)
        {
            mOnEvent += onEvent;
            return new CustomUnRegister(() => { UnRegister(onEvent); });
        }

        public void UnRegister(Action<T, TK> onEvent) => mOnEvent -= onEvent;

        public void Trigger(T t, TK k) => mOnEvent?.Invoke(t, k);

        IUnRegister IEasyEvent.Register(Action onEvent)
        {
            return Register(Action);
            void Action(T _, TK __) => onEvent();
        }
    }

    public class EasyEvent<T, TK, TS> : IEasyEvent
    {
        private Action<T, TK, TS> mOnEvent = (_, _, _) => { };

        public IUnRegister Register(Action<T, TK, TS> onEvent)
        {
            mOnEvent += onEvent;
            return new CustomUnRegister(() => { UnRegister(onEvent); });
        }

        public void UnRegister(Action<T, TK, TS> onEvent) => mOnEvent -= onEvent;

        public void Trigger(T t, TK k, TS s) => mOnEvent?.Invoke(t, k, s);

        IUnRegister IEasyEvent.Register(Action onEvent)
        {
            return Register(Action);
            void Action(T _, TK __, TS ___) => onEvent();
        }
    }

    public class EasyEvents
    {
        private static readonly EasyEvents mGlobalEvents = new EasyEvents();

        public static T Get<T>() where T : IEasyEvent => mGlobalEvents.GetEvent<T>();

        public static void Register<T>() where T : IEasyEvent, new() => mGlobalEvents.AddEvent<T>();

        private readonly Dictionary<Type, IEasyEvent> mTypeEvents = new Dictionary<Type, IEasyEvent>();

        public void AddEvent<T>() where T : IEasyEvent, new() => mTypeEvents.Add(typeof(T), new T());

        public T GetEvent<T>() where T : IEasyEvent
        {
            return mTypeEvents.TryGetValue(typeof(T), out var e) ? (T)e : default;
        }

        public T GetOrAddEvent<T>() where T : IEasyEvent, new()
        {
            var eType = typeof(T);
            if (mTypeEvents.TryGetValue(eType, out var e))
            {
                return (T)e;
            }

            var t = new T();
            mTypeEvents.Add(eType, t);
            return t;
        }
    }

    #endregion


    #region Event Extension

    public class OrEvent : IUnRegisterList
    {
        public OrEvent Or(IEasyEvent easyEvent)
        {
            easyEvent.Register(Trigger).AddToUnregisterList(this);
            return this;
        }

        private Action mOnEvent = () => { };

        public IUnRegister Register(Action onEvent)
        {
            mOnEvent += onEvent;
            return new CustomUnRegister(() => { UnRegister(onEvent); });
        }

        public IUnRegister RegisterWithACall(Action onEvent)
        {
            onEvent.Invoke();
            return Register(onEvent);
        }
        
        public void UnRegister(Action onEvent)
        {
            mOnEvent -= onEvent;
            this.UnRegisterAll();
        }

        private void Trigger() => mOnEvent?.Invoke();

        public List<IUnRegister> UnregisterList { get; } = new List<IUnRegister>();
    }

    public static class OrEventExtensions
    {
        public static OrEvent Or(this IEasyEvent self, IEasyEvent e) => new OrEvent().Or(self).Or(e);
    }

    #endregion

#if UNITY_EDITOR
    internal class EditorMenus
    {
        [UnityEditor.MenuItem("QFramework/Install QFrameworkWithToolKits")]
        public static void InstallPackageKit() => Application.OpenURL("https://qframework.cn/qf");
    }
#endif
}