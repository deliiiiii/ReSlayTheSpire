using System;
using System.Linq.Expressions;

namespace BehaviourTree
{
    public static class BlackboardExt
    {
        public static TField Get<T, TField>(this T board, string fieldName)
        {
            var param = Expression.Parameter(board.GetType(), "x");
            var field = Expression.Field(param, fieldName);
            var lambda = Expression.Lambda<Func<T, TField>>(field, param);
            return lambda.Compile()(board);
        }   
        
        // public static TField Get<T, TField>(this T board, string fieldName)
        // {
        //     var param = Expression.Parameter(board.GetType(), "x");
        //     var field = Expression.Field(param, fieldName);
        //     var lambda = Expression.Lambda<Func<T, TField>>(field, param);
        //     return lambda.Compile()(board);
        // }   
        //
        
        // Int => typeof(int) => CreateFieldGetter<BlackboardTest, int>("Int");

        // <int, CreateFieldGetter<BlackboardTest, int>>

        // <T, CreateFieldGetter<BlackboardTest, T>> dic

        // dic[typeof(int)] = CreateFieldGetter<BlackboardTest, int> -- Func<string, Func<BlackboardTest, int>>
        
        // dic[typeof(T)] = CreateFieldGetter<BlackboardTest, T> -- Func<string, Func<BlackboardTest, T>>

        // dic -- Dictionary<Type, Func<string, Func<BlackboardTest, T>>>

        // 包装一下object Dictionary<Type, Func<string, Func<Blackboard, IObservable<object>>>>

        //  FuncName<T> -> Func<string, Func<Blackboard, IObservable<T>>>

        // FuncName<T>(string) -> Func<Blackboard, IObservable<T>>

        // (typeof(int), getter<int>(board, name) === Dic<Type, Func<Blackboard, string, object>>
    }
}