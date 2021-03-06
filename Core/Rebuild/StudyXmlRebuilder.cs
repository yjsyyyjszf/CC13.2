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
using MatrixPACS.Dicom.Utilities.Xml;
using MatrixPACS.ImageServer.Common.Exceptions;
using MatrixPACS.ImageServer.Core.Command;
using MatrixPACS.ImageServer.Core.Process;
using MatrixPACS.ImageServer.Core.Validation;
using MatrixPACS.ImageServer.Enterprise.Command;
using MatrixPACS.ImageServer.Model;
using UpdateStudySizeInDBCommand = MatrixPACS.ImageServer.Core.Command.UpdateStudySizeInDBCommand;

namespace MatrixPACS.ImageServer.Core.Rebuild
{
	/// <summary>
	/// Class for rebuilding StudyXml files based on the current contents of the <see cref="StudyXml"/> file.
	/// </summary>
	public class StudyXmlRebuilder
	{
		#region Private Members

		private readonly StudyStorageLocation _location;

        #endregion

        #region Constructor

        /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="location">The location of the Study to rebuild.</param>
		public StudyXmlRebuilder(StudyStorageLocation location)
		{
			_location = location;
		}

		#endregion

		/// <summary>
		/// Do the actual rebuild.  On error, will attempt to reprocess the study.
		/// </summary>
		public void RebuildXml()
		{
			string rootStudyPath = _location.GetStudyPath();

			try
			{
				using (ServerCommandProcessor processor = new ServerCommandProcessor("Rebuild XML"))
				{
				    var command = new RebuildStudyXmlCommand(_location.StudyInstanceUid, rootStudyPath);
					processor.AddCommand(command);

                    var updateCommand = new UpdateStudySizeInDBCommand(_location, command);
                    processor.AddCommand(updateCommand);

					if (!processor.Execute())
					{
						throw new ApplicationException(processor.FailureReason, processor.FailureException);
					}

                    Study theStudy = _location.Study;
                    if (theStudy.NumberOfStudyRelatedInstances != command.StudyXml.NumberOfStudyRelatedInstances)
                    {
                        // We rebuilt, but the counts don't match.
                        throw new StudyIntegrityValidationFailure(ValidationErrors.InconsistentObjectCount,
                                                                  new ValidationStudyInfo(theStudy,
                                                                                          _location.ServerPartition),
                                                                  string.Format(
                                                                      "Database study count {0} does not match study xml {1}",
                                                                      theStudy.NumberOfStudyRelatedInstances,
                                                                      command.StudyXml.NumberOfStudyRelatedInstances));
                    }

					Platform.Log(LogLevel.Info, "Completed reprocessing Study XML file for study {0}", _location.StudyInstanceUid);
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Unexpected error when rebuilding study XML for directory: {0}",
				             _location.FilesystemPath);
				StudyReprocessor reprocessor = new StudyReprocessor();
                try
                {
                    WorkQueue reprocessEntry = reprocessor.ReprocessStudy("Rebuild StudyXml", _location, Platform.Time);
                    if (reprocessEntry != null)
                    {
                        Platform.Log(LogLevel.Error, "Failure attempting to reprocess study: {0}",
                                     _location.StudyInstanceUid);
                    }
                    else
                        Platform.Log(LogLevel.Error, "Inserted reprocess request for study: {0}",
                                     _location.StudyInstanceUid);
                }
                catch(InvalidStudyStateOperationException ex)
                {
                    Platform.Log(LogLevel.Error, "Failure attempting to reprocess study {0}: {1}",
                                     _location.StudyInstanceUid, ex.Message);
                }
			}
		}
	}
}