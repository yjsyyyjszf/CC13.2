#region License

// Copyright (c) 2013, MatrixPACS Inc.
// All rights reserved.
// http://www.MatrixPACS.ca
//
// This file is part of the MatrixPACS RIS/PACS open source project.
//
// The MatrixPACS RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The MatrixPACS RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the MatrixPACS RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System;
using MatrixPACS.Common;
using MatrixPACS.Common.Utilities;
using MatrixPACS.Enterprise.Core;
using MatrixPACS.ImageServer.Common;
using MatrixPACS.ImageServer.Model;

namespace MatrixPACS.ImageServer.Services.Archiving.Hsm
{
	/// <summary>
	/// Service thread for handling archivals.
	/// </summary>
	public class HsmArchiveService : ThreadedService
	{
		private readonly HsmArchive _hsmArchive;
		private readonly ItemProcessingThreadPool<ArchiveQueue> _threadPool;

		public HsmArchiveService(string name, HsmArchive hsmArchive) : base(name)
		{
			_hsmArchive = hsmArchive;
			_threadPool = new ItemProcessingThreadPool<ArchiveQueue>(HsmSettings.Default.ArchiveThreadCount);
			_threadPool.ThreadPoolName = "HsmArchive Pool";
		}

		protected override bool Initialize()
		{
			_hsmArchive.ResetFailedArchiveQueueItems();

			// Start the thread pool
			if (!_threadPool.Active)
				_threadPool.Start();

            return true;
		}

		/// <summary>
		/// Execute the service.
		/// </summary>
		protected override void Run()
		{
			while (true)
			{
				if ((_threadPool.QueueCount + _threadPool.ActiveCount) < _threadPool.Concurrency)
				{
					try
					{
						ArchiveQueue queueItem = _hsmArchive.GetArchiveCandidate();

						if (queueItem != null)
						{
							HsmStudyArchive archiver = new HsmStudyArchive(_hsmArchive);
							_threadPool.Enqueue(queueItem, archiver.Run);
						}
						else if (CheckStop(HsmSettings.Default.PollDelayMilliseconds))
						{
							Platform.Log(LogLevel.Info, "Shutting down {0} archiving service.", _hsmArchive.PartitionArchive.Description);
							return;
						}
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Error,e,"Unexpected exception when querying for archive candidates, rescheduling.");
						if (CheckStop(HsmSettings.Default.PollDelayMilliseconds))
						{
							Platform.Log(LogLevel.Info, "Shutting down {0} archiving service.", _hsmArchive.PartitionArchive.Description);
							return;
						}
					}
				}
				else
				{
					if (CheckStop(HsmSettings.Default.PollDelayMilliseconds))
					{
						Platform.Log(LogLevel.Info, "Shutting down {0} archiving service.", _hsmArchive.PartitionArchive.Description);
						return;
					}
				}
			}
		}

		/// <summary>
		/// Stop the service.
		/// </summary>
		protected override void Stop()
		{
			//TODO CR (Jan 2014): Move this into the base if it applies to all subclasses?
			PersistentStoreRegistry.GetDefaultStore().ShutdownRequested = true;

			_threadPool.Stop(true);
		}
	}
}