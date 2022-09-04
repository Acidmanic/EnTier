using System;
using System.Collections.Generic;
using System.Linq;
using Acidmanic.Utilities.Reflection.Extensions;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Acidmanic.Utilities.Reflection.ObjectTree.FieldAddressing;
using EnTier.Utility;
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

            var evaluator = new ObjectEvaluator(entityType);
            
            var allLeaves = evaluator.RootNode.EnumerateLeavesBelow();

            var idLeaf = IdHelper.GetIdLeaf(entityType);

            FieldKey idKey = GetKey(idLeaf, evaluator, allLeaves);
            
            IdField = new ExpectedFieldProperties(entityType.GetUniqueIdFieldLeaf(),idKey);

            AutogeneratedFields = entityType.ListAutoGeneratedFields()
                .Select(leaf =>
                {
                    var uKey = GetKey(leaf, evaluator, allLeaves);
                    
                    return new ExpectedFieldProperties(leaf,uKey);
                    
                })
                .ToList();
        }

        private FieldKey GetKey(AccessNode idLeaf, ObjectEvaluator evaluator, List<AccessNode> allLeaves)
        {
            if (idLeaf == null)
            {
                return null;
            }

            var fullName = idLeaf.GetFullName();

            var corresponding  = allLeaves.FirstOrDefault(l => l.GetFullName() == fullName);

            if (corresponding == null)
            {
                return null;
            }

            return evaluator.Map.FieldKeyByNode(corresponding);

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