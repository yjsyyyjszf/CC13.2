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

using MatrixPACS.ImageServer.Enterprise;

namespace MatrixPACS.ImageServer.Model.Parameters
{
    public class SetStudyRelatedInstanceCountParameters : ProcedureParameters
    {
        public SetStudyRelatedInstanceCountParameters(ServerEntityKey studyStorageKey)
            : base("SetStudyRelatedInstanceCount")
        {
            StudyStorageKey = studyStorageKey;
        }

        public ServerEntityKey StudyStorageKey
        {
            set { SubCriteria["StudyStorageKey"] = new ProcedureParameter<ServerEntityKey>("StudyStorageKey", value); }
        }

        
        public int StudyRelatedInstanceCount
        {
            set { SubCriteria["StudyRelatedInstanceCount"] = new ProcedureParameter<int>("StudyRelatedInstanceCount", value); }
        }

        public int StudyRelatedSeriesCount
        {
            set { SubCriteria["StudyRelatedSeriesCount"] = new ProcedureParameter<int>("StudyRelatedSeriesCount", value); }
        }
    }
}