﻿using EnvDTE;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Karpach.VisualStudio.Launcher
{
	public class VisualStudioLocator
	{
		[DllImport("ole32.dll")]
		private static extern int GetRunningObjectTable(int reserved, out UCOMIRunningObjectTable prot);

		[DllImport("ole32.dll")]
		private static extern int CreateBindCtx(int reserved, out UCOMIBindCtx ppbc);

		/// <summary>
		/// Get a snapshot of the running object table (ROT).
		/// </summary>
		/// <returns>A hashtable mapping the name of the object in the ROT to the corresponding object</returns>
		private static Hashtable GetRunningObjectTable()
		{
			Hashtable result = new Hashtable();

			int numFetched;
			UCOMIRunningObjectTable runningObjectTable;
			UCOMIEnumMoniker monikerEnumerator;
			UCOMIMoniker[] monikers = new UCOMIMoniker[1];

			GetRunningObjectTable(0, out runningObjectTable);
			runningObjectTable.EnumRunning(out monikerEnumerator);
			monikerEnumerator.Reset();

			while (monikerEnumerator.Next(1, monikers, out numFetched) == 0)
			{
				UCOMIBindCtx ctx;
				CreateBindCtx(0, out ctx);

				string runningObjectName;
				monikers[0].GetDisplayName(ctx, null, out runningObjectName);

				object runningObjectVal;
				runningObjectTable.GetObject(monikers[0], out runningObjectVal);

				result[runningObjectName] = runningObjectVal;
			}

			return result;
		}

		/// <summary>
		/// Get a table of the currently running instances of the Visual Studio .NET IDE.
		/// </summary>
		/// <param name="openSolutionsOnly">Only return instances that have opened a solution</param>
		/// <returns>A hashtable mapping the name of the IDE in the running object table to the corresponding DTE object</returns>
		public static _DTE[] GetIDEInstances(bool openSolutionsOnly)
		{
			var runningIDEInstances = new Dictionary<string, _DTE>();
			Hashtable runningObjects = GetRunningObjectTable();

			IDictionaryEnumerator rotEnumerator = runningObjects.GetEnumerator();
			while (rotEnumerator.MoveNext())
			{
				string candidateName = (string)rotEnumerator.Key;
				if (candidateName != null && !candidateName.StartsWith("!VisualStudio.DTE"))
				{
					continue;
				}

				_DTE ide = rotEnumerator.Value as _DTE;
				if (ide == null)
				{
					continue;
				}

				if (openSolutionsOnly)
				{
					try
					{
						string solutionFile = ide.Solution.FullName;
						if (!string.IsNullOrEmpty(solutionFile))
						{
							runningIDEInstances[candidateName] = ide;
						}
					}
					catch { }
				}
				else
				{
					runningIDEInstances[candidateName] = ide;
				}
			}
			return runningIDEInstances.Values.ToArray();
		}
	}
}