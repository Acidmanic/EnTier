﻿using System;
using System.Linq;
using System.Threading.Tasks;
using EnTier;
using EnTier.Controllers;
using EnTier.UnitOfWork;
using ExampleEntityFramework.DomainModels;
using ExampleEntityFramework.EventStream;
using ExampleEntityFramework.StoragesModels;
using ExampleEntityFramework.TransferModels;
using Microsoft.AspNetCore.Mvc;

namespace ExampleEntityFramework.Controllers
{
    
    [ApiController]
    [Route("post-events")]
    public class PostEventsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PostEventsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Route("update-title/{postId}")]
        public  async Task<IActionResult> UpdatePostTitle(long postId, UpdateDto titleUpdate)
        {
            var repository = _unitOfWork.GetStreamRepository<IPostEvent, long, long>();

            var @event = new ChangePostTitleEvent
            {
                PostId = postId,
                Timestamp = DateTime.Now.Ticks,
                Title = titleUpdate.Value
            };

            var eventId = await repository.Append(@event, postId);

            _unitOfWork.Complete();
            
            if (!eventId)
            {
                return BadRequest();
            }

            
            return Ok(eventId);
        }
        
        
        [HttpGet]
        [Route("stream/{postId}")]
        public  async Task<IActionResult> ReadStreamByStreamId(long postId)
        {
            var repository = _unitOfWork.GetStreamRepository<IPostEvent, long, long>();

            var events = await repository.ReadStream(postId);
            
            return Ok(new {
                events = events.Select( e=> e as object)
            });
        }
        
        [HttpGet]
        [Route("stream")]
        public  async Task<IActionResult> ReadStreamByStreamId()
        {
            var repository = _unitOfWork.GetStreamRepository<IPostEvent, long, long>();

            var events = await repository.ReadStream();
            
            return Ok(new {
                events = events.Select( e=> e as object)
            });
        }
        
        [HttpGet]
        [Route("stream-chunk/{postId}")]
        public  async Task<IActionResult> ReadChunkByStreamId(long postId,[FromQuery] long baseEventId =0,[FromQuery] long count = 50)
        {
            var repository = _unitOfWork.GetStreamRepository<IPostEvent, long, long>();

            var events = await repository.ReadStreamChunk(postId,baseEventId,count);
            
            return Ok(new {
                events = events.Select(e => e as object)
            });
        }
        
        [HttpGet]
        [Route("stream-chunk")]
        public  async Task<IActionResult> ReadAllChunk([FromQuery] long baseEventId =0,[FromQuery] long count = 50)
        {
            var repository = _unitOfWork.GetStreamRepository<IPostEvent, long, long>();

            var events = await repository.ReadStreamChunk(baseEventId,count);
            
            return Ok(new {
                events = events.Select(e => e as object)
            });
        }
    }
}