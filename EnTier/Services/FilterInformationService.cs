using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acidmanic.Utilities.Filtering.Extensions;
using Acidmanic.Utilities.Reflection.ObjectTree;
using EnTier.Attributes;
using EnTier.Contracts;
using EnTier.Extensions;
using EnTier.Models;
using EnTier.UnitOfWork;

namespace EnTier.Services;

public class FilterInformationService:IFilterInformationService
{

    private readonly IUnitOfWork _unitOfWork;

    public FilterInformationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<FilterProfile>> GetFilterProfileAsync<TStorage,TId>(bool fullTree) where TStorage : class, new()
    {
        var ev = new ObjectEvaluator(typeof(TStorage));

        IEnumerable<AccessNode> leaves =
            fullTree
                ? ev.Map.Nodes.Where(n => n.IsLeaf && n.IsFilterField())
                : ev.RootNode.GetDirectLeaves().Where(n => n.IsFilterField());

        Func<AccessNode, string> getAddress =
            fullTree ? n => ev.Map.FieldKeyByNode(n).Headless().ToString() : n => n.Name;

        var filteringProfile = new List<FilterProfile>();

        foreach (var leaf in leaves)
        {
            var byCollectionAttribute = leaf.PropertyAttributes
                .FirstOrDefault(a => a is FilterByCollectionValuesAttribute);

            var availableValues = new List<string>();
            
            var repository = _unitOfWork.GetCrudRepository<TStorage, TId>();

            var leafAddress = getAddress(leaf);

            var range = await repository.GetFilterRangeAsync(leafAddress);
            
            if (byCollectionAttribute is FilterByDefinedValuesAttribute defined)
            {
                availableValues = defined.ValuesCollection;
            }

            if (byCollectionAttribute is FilterByExistingValuesAttribute)
            {
                availableValues = await repository.GetExistingValuesAsync(leafAddress);
            }

            var profile = new FilterProfile
            {
                FieldName = getAddress(leaf),
                Es6Type = leaf.GetEffectiveEs6Type(),
                AvailableValues = availableValues,
                FilterByAvailableValues = byCollectionAttribute != null,
                FullTreeAccess = fullTree,
                MaximumValue = range.Maximum,
                MinimumValue = range.Minimum
            };

            filteringProfile.Add(profile);
        }

        return filteringProfile;
    }

    public List<FilterProfile> GetFilterProfile<TStorage, TId>(bool fullTree) where TStorage : class, new()
    {
        return GetFilterProfileAsync<TStorage, TId>(fullTree).Result;
    }
}