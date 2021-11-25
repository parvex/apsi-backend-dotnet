﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apsi.Database;
using Apsi.Database.Entities;
using apsi.backend.social.Models;
using apsi.backend.social.Services;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Apsi.Backend.Social.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IUserService _userService;
        private readonly ISocialGroupService _socialGroupService;
        private readonly AppDbContext _context;


        public PostController(IUserService userService, IPostService postService, ISocialGroupService socialGroupService, AppDbContext context)
        {
            _userService = userService;
            _socialGroupService = socialGroupService;
            _postService = postService;
            _context = context;
        }

        [HttpGet("GetPostsByAuthor")]
        public async Task<ActionResult<List<PostDto>>> GetPostsByAuthor([FromQuery] AuthorPagingDto authorPaging)
        {
            return await _postService.GetPostsByAuthor(authorPaging);
        }

        [HttpGet("GetPostsById")]
        public async Task<ActionResult<PostDto>> GetPostById([FromQuery] IdPagingDto idPaging)
        {
            return await _postService.GetPostById(idPaging);
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<PostDto>>> GetAllPosts([FromQuery] PagingDto paging)
        {
            return await _postService.GetAll(paging);
        }

        [HttpPost("CreatePost")]
        public async Task<ActionResult<int>> CreatePost(CreatePostDto post)
        {
            var name = ClaimTypes.Name;
            if(name == null)
            {
                return BadRequest("Post not created, no user");
            }
            else
            {
                var dbUser = await _userService.GetUserById(int.Parse(HttpContext.User.Identity.Name));
                var dbSocialGroup = await _socialGroupService.GetDbDataByName(post.socialGroupName);;
                var postId = await _postService.CreatePost(post, dbUser, dbSocialGroup);
                if (postId == null)
                {
                    return BadRequest("Post not created");
                }
                else
                {
                    return Ok(postId);
                }
            }
        }
    }
}