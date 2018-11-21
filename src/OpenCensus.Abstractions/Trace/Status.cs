// <copyright file="Status.cs" company="OpenCensus Authors">
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
        public static readonly Status Ok = new Status(CanonicalCode.Ok);
        public static readonly Status Cancelled = new Status(CanonicalCode.Cancelled);
        public static readonly Status Unknown = new Status(CanonicalCode.Unknown);
        public static readonly Status InvalidArgument = new Status(CanonicalCode.InvalidArgument);
        public static readonly Status DeadlineExceeded = new Status(CanonicalCode.DeadlineExceeded);
        public static readonly Status NotFound = new Status(CanonicalCode.NotFound);
        public static readonly Status AlreadyExists = new Status(CanonicalCode.AlreadyExists);
        public static readonly Status PermissionDenied = new Status(CanonicalCode.PermissionDenied);
        public static readonly Status Unauthenticated = new Status(CanonicalCode.Unauthenticated);
        public static readonly Status ResourceExhausted = new Status(CanonicalCode.ResourceExhausted);
        public static readonly Status FailedPrecondition = new Status(CanonicalCode.FailedPrecondition);
        public static readonly Status Aborted = new Status(CanonicalCode.Aborted);
        public static readonly Status OutOfRange = new Status(CanonicalCode.OutOfRange);
        public static readonly Status Unimplemented = new Status(CanonicalCode.Unimplemented);
        public static readonly Status Internal = new Status(CanonicalCode.Internal);
        public static readonly Status Unavailable = new Status(CanonicalCode.Unavailable);
        public static readonly Status DataLoss = new Status(CanonicalCode.DataLoss);

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
                return this.CanonicalCode == CanonicalCode.Ok;
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int result = 1;
            result = (31 * result) + this.CanonicalCode.GetHashCode();
            result = (31 * result) + this.Description.GetHashCode();
            return result;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "Status{"
                    + "canonicalCode=" + this.CanonicalCode + ", "
                    + "description=" + this.Description
                    + "}";
        }
    }
}
