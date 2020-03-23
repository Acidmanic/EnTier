


using System;
using System.Collections.Generic;
using Context;
using Plugging;
using StorageModels;

namespace Repository{



    public class ProjectsRepository : IProjectsRepository
    {


        private IObjectMapper _objectMapper;

        private IDataset<Project> _dataset;


        public ProjectsRepository(IDatasetAccessor datasetAccessor)
        {
            _dataset = datasetAccessor.Get<Project>();
        }


        public Project Add(Project value)
        {
            throw new NotImplementedException();
        }

        public List<Project> Find(Func<Project, bool> condition)
        {
            throw new NotImplementedException();
        }

        public List<Project> GetAll()
        {
            throw new NotImplementedException();
        }

        public Project GetById(long id)
        {

            string desc = "Data set is null";

            if(_dataset != null){
                desc = _dataset.GetType().FullName;
            }

            return new Project{
                Name="Received Dataset",
                Description = desc
            };
        }

        public Project GetById(Project entity)
        {
            throw new NotImplementedException();
        }

        public Project Remove(Project value)
        {
            throw new NotImplementedException();
        }

        public Project RemoveById(long id)
        {
            throw new NotImplementedException();
        }
    }
}