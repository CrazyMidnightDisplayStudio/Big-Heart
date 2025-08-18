// #if UNITY_EDITOR
// using System;
// using System.Collections.Generic;
// using Services.Inventory;
// namespace Services
// {
//     public static partial class ServiceRegistry
//     {
//         // ───────────── Editor-only: фабрика стабов ─────────────
//         private static class EditorStubs
//         {
//             private static readonly Dictionary<Type, Func<object>> _factories = new Dictionary<Type, Func<object>>
//             {
//                 {typeof(ICoroutineService), () => new StubCoroutineService()},
//                 {typeof(IDateProgressService), () => new StubDateProgressService()},
//                 {typeof(IEventService), () => new StubEventService()},
//                 {typeof(IInventoryService), () => new StubInventoryService()},
//                 {typeof(IItemService), () => new StubItemService()}
//             };
//
//             public static bool TryGetStub(Type t, out object stub)
//             {
//                 if (_factories.TryGetValue(t, out var f))
//                 {
//                     stub = f();
//                     return true;
//                 }
//                 stub = null;
//                 return false;
//             }
//         }
//     }
// }
// #endif
