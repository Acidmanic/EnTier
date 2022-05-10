using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EnTier.Repositories;
using Xunit;

namespace Entier.Test.Unit
{
    public class RepositoryBaseTests
    {
        private class Property
        {
            public string Name { get; set; }
        }

        private class Model
        {
            public long Id { get; set; }

            public Property Property { get; set; }
        }

        private class DummyRepo : CrudRepositoryBase<Model, long>
        {
            public override IEnumerable<Model> All()
            {
                throw new NotImplementedException();
            }

            public override Model Add(Model value)
            {
                throw new NotImplementedException();
            }

            public override Model GetById(long id)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<Model> Find(Expression<Func<Model, bool>> predicate)
            {
                throw new NotImplementedException();
            }

            public override bool Remove(Model value)
            {
                throw new NotImplementedException();
            }

            public override bool Remove(long id)
            {
                throw new NotImplementedException();
            }

            public TEntity ExposedStrip<TEntity>(TEntity value)
                where TEntity : class, new()
            {
                return base.StripNonPrimitives(value);
            }
        }

        [Fact]
        public void StripMethodShouldRemoveAllNonPrimitiveProperties()
        {
            var repo = new DummyRepo();

            var model = new Model
            {
                Id = 100,
                Property = new Property
                {
                    Name = "Pashmak"
                }
            };

            Model stripped = repo.ExposedStrip(model);

            Assert.NotNull(stripped);

            Assert.Equal(model.Id, stripped.Id);
            
            Assert.Null(stripped.Property);
        }
    }
}