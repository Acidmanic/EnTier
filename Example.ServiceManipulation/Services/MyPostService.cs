using System.Collections.Generic;
using System.Linq;
using EnTier;
using EnTier.Mapper;
using EnTier.Regulation;
using EnTier.Services;
using EnTier.UnitOfWork;
using ServiceManipulationExample.Models;

namespace ServiceManipulationExample.Services
{
    public class MyPostService:CrudService<Post,Post,long,long>
    {
    

        public override IEnumerable<Post> GetAll()
        {
            var result =  base.GetAll().Select(Manipulate);

            return result;
        }

        public override Post GetById(long id)
        {
            return Manipulate(base.GetById(id));
        }

        private Post Manipulate(Post p)
        {
            if (p == null)
            {
                return new Post
                {
                    Title = "It Was A Null!",
                    Content = "This post does not exist! It was a null object i took its place!"
                };
            }
            p.Content = "[Manipulated in MYYY Service!!!] " + p.Content;
            
            return p;
        }

        public MyPostService(EnTierEssence essence) : base(essence)
        {
        }
    }
}