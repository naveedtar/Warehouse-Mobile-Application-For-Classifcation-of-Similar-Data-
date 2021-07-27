// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using Android.App;
// using Android.Content;
// using Android.OS;
// using Android.Runtime;
// using Android.Views;
// using Android.Widget;
// using WarehouseHandheld.Droid.Helpers;
// using WarehouseHandheld.Helpers;
// using Xamarin.Forms;
//
// [assembly: Dependency(typeof(CrashReporting))]
// namespace WarehouseHandheld.Droid.Helpers
// {
//     public class CrashReporting : ICrashReporting
//     {
//         /// <summary>
//         /// Crashalytics reporting initialize 
//         /// </summary>
//         /// <returns><c>true</c>, if reporting init failed, <c>false</c> otherwise worked OK.</returns>
//         public bool CrashReportingInit()
//         {
//             try
//             {
//                 var context = Android.App.Application.Context;
//                 //Fabric.Fabric.With(context, new Crashlytics.Crashlytics());
//             }
//             catch (Exception exception)
//             {
//                 System.Diagnostics.Debug.WriteLine("CrashReportingInit - " +
//                                         " failed - " + exception.Message);
//                 return true;
//             }
//
//             return false;
//         }
//
//         /// <summary>
//         /// Sets Misc options for crash reporting
//         /// </summary>
//         /// <returns><c>true</c>, if reporting misc failed, <c>false</c> otherwise worked OK.</returns>
//         public bool CrashReportingMisc()
//         {
//             try
//             {
//                 // Optional: Setup Xamarin / Mono Unhandled exception parsing / handling
//                 //Crashlytics.Crashlytics.HandleManagedExceptions();
//                 //CrashReportingMiscDone = true;
//             }
//             catch (Exception exception)
//             {
//                 System.Diagnostics.Debug.WriteLine("CrashReportingMisc - " +
//                                         " failed - " + exception.Message);
//                 return true;
//             }
//
//             return false;
//         }
//
//         /// <summary>
//         /// Forces an application crash 
//         /// </summary>
//         public void ForceCrash()
//         {
//             //Crashlytics.Crashlytics.Instance.Crash();
//             throw new ApplicationException("This is a forced crash");
//         }
//     }
// }