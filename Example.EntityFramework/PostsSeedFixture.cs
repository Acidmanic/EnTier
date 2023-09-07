using EnTier.DataAccess.JsonFile;
using EnTier.Extensions;
using EnTier.Repositories;
using EnTier.UnitOfWork;
using ExampleEntityFramework.StoragesModels;

namespace ExampleEntityFramework;

public class PostsSeedFixture
{

    public void Setup(ICrudRepository<MarkedFilterResult<PostStg, long>,long> repository)
    {
        var existing = repository.All();

        foreach (var item in existing)
        {
            repository.Remove(item.Id);
        }
    }
    
    public void Setup(ICrudRepository<PostStg, long> repository,IUnitOfWork unitOfWork)
    {

        var existing = repository.All();

        foreach (var postStg in existing)
        {
            repository.Remove(postStg.Id);
        }
        
        unitOfWork.Complete();
        
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
        
        unitOfWork.Complete();
        
        unitOfWork.UpdateIndexes<PostStg,long>(false);
        
    }
}