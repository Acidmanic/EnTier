using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection.Extensions;
using Meadow.Reflection.Conventions;
using Meadow.Requests;

namespace EnTier.DataAccess.Meadow.GenericCrudRequests
{
    public abstract class StorageRequest<TStorageIn, TStorageOut> : MeadowRequest<TStorageIn, TStorageOut>
        where TStorageOut : class, new()
    {
        protected NameConvention NameConvention { get; }


        internal ExpectedFieldProperties IdField { get; }

        internal List<ExpectedFieldProperties> AutogeneratedFields { get; }

        //Main Construction Entry
        public StorageRequest(Type entityType) : base(true)
        {
            NameConvention = new NameConvention(entityType);

            IdField = new ExpectedFieldProperties(entityType.GetUniqueIdFieldLeaf());

            AutogeneratedFields = entityType.ListAutoGeneratedFields()
                .Select(leaf => new ExpectedFieldProperties(leaf))
                .ToList();
        }

        protected abstract string PickProcedureName(NameConvention nameConvention);

        public override string RequestText
        {
            get
            {
                return PickProcedureName(NameConvention);
            }
            protected set{}
        }

        protected override bool FullTreeReadWrite()
        {
            return false;
        }
    }
}