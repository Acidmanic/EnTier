using Acidmanic.Utilities.Filtering.Models;
using EnTier.Repositories;
using ExampleEntityFramework.StoragesModels;

namespace ExampleEntityFramework;

public class PostsSeedFixture
{

    public void Setup(ICrudRepository<FilterResult, long> repository)
    {
        var existing = repository.All();

        foreach (var item in existing)
        {
            repository.Remove(item.Id);
        }
    }
    
    public void Setup(ICrudRepository<PostStg, long> repository)
    {

        var existing = repository.All();

        foreach (var postStg in existing)
        {
            repository.Remove(postStg.Id);
        }
        
        repository.Add(new PostStg
        {
            Content = "First Content",
            Title = "First"
        });
        repository.Add(new PostStg
        {
            Content = "Second Content",
            Title = "Second"
        });
        repository.Add(new PostStg
        {
            Content = "Third Content",
            Title = "Third"
        });
        repository.Add(new PostStg
        {
            Content = "Fourth Content",
            Title = "Fourth"
        });
        repository.Add(new PostStg
        {
            Content = "Fifth Content",
            Title = "Fifth"
        });
    }
}