// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public sealed class ParamsSymbolContext
    {
        private readonly ParamsSemanticModel paramsSemanticModel;
        private bool unlocked;

        public ParamsSymbolContext(ParamsSemanticModel paramsSemanticModel)
        {
            this.paramsSemanticModel = paramsSemanticModel;
        }

        public ParamsTypeManager ParamsTypeManager
        {
            get
            {
                this.CheckLock();
                return this.paramsSemanticModel.ParamsTypeManager;
            }
        }

        public void Unlock() => this.unlocked = true;

        private void CheckLock()
        {
            if (this.unlocked == false)
            {
                throw new InvalidOperationException("Properties of the symbol context should not be accessed until name binding is completed.");
            }
        }
    }
}
