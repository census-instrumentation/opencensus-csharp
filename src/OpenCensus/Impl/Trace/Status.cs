﻿// <copyright file="Status.cs" company="OpenCensus Authors">
// Copyright 2018, OpenCensus Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of theLicense at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

namespace OpenCensus.Trace
{
    public class Status
    {
        public static readonly Status Ok = new Status(CanonicalCode.OK);
        public static readonly Status Cancelled = new Status(CanonicalCode.CANCELLED);
        public static readonly Status Unknown = new Status(CanonicalCode.UNKNOWN);
        public static readonly Status InvalidArgument = new Status(CanonicalCode.INVALID_ARGUMENT);
        public static readonly Status DeadlineExceeded = new Status(CanonicalCode.DEADLINE_EXCEEDED);
        public static readonly Status NotFound = new Status(CanonicalCode.NOT_FOUND);
        public static readonly Status AlreadyExists = new Status(CanonicalCode.ALREADY_EXISTS);
        public static readonly Status PermissionDenied = new Status(CanonicalCode.PERMISSION_DENIED);
        public static readonly Status Unauthenticated = new Status(CanonicalCode.UNAUTHENTICATED);
        public static readonly Status ResourceExhausted = new Status(CanonicalCode.RESOURCE_EXHAUSTED);
        public static readonly Status FailedPrecondition = new Status(CanonicalCode.FAILED_PRECONDITION);
        public static readonly Status Aborted = new Status(CanonicalCode.ABORTED);
        public static readonly Status OutOfRange = new Status(CanonicalCode.OUT_OF_RANGE);
        public static readonly Status Unimplemented = new Status(CanonicalCode.UNIMPLEMENTED);
        public static readonly Status Internal = new Status(CanonicalCode.INTERNAL);
        public static readonly Status Unavailable = new Status(CanonicalCode.UNAVAILABLE);
        public static readonly Status DataLoss = new Status(CanonicalCode.DATA_LOSS);

        internal Status(CanonicalCode canonicalCode, string description = null)
        {
            this.CanonicalCode = canonicalCode;
            this.Description = description;
        }

        public CanonicalCode CanonicalCode { get; }

        public string Description { get; }

        public bool IsOk
        {
            get
            {
                return CanonicalCode.OK == this.CanonicalCode;
            }
        }

        public Status WithDescription(string description)
        {
            if (this.Description == description)
            {
                return this;
            }

            return new Status(this.CanonicalCode, description);
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            if (!(obj is Status))
            {
                return false;
            }

            Status that = (Status)obj;
            return this.CanonicalCode == that.CanonicalCode && this.Description == that.Description;
        }

        public override int GetHashCode()
        {
            int result = 1;
            result = (31 * result) + this.CanonicalCode.GetHashCode();
            result = (31 * result) + this.Description.GetHashCode();
            return result;
        }

        public override string ToString()
        {
            return "Status{"
                    + "canonicalCode=" + this.CanonicalCode + ", "
                    + "description=" + this.Description
                    + "}";
        }
    }
}
