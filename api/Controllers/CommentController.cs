using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using api.DTOs.Comment;
using api.Repositories;
using api.Interfaces;
using api.Models;

namespace api.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICommentRepository _commentRepo;
        private readonly IStockRepository _stockRepo;

        public CommentController(ICommentRepository commentRepo, IStockRepository stockRepo, IMapper mapper)
        {
            _commentRepo = commentRepo;
            _stockRepo = stockRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var comments = await _commentRepo.GetAllAsync();
            var commentDTOs = _mapper.Map<List<CommentDTO>>(comments);
            return Ok(commentDTOs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var comment = await _commentRepo.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound("Comment not found");
            }
            var commentDTO = _mapper.Map<CommentDTO>(comment);
            return Ok(commentDTO);
        }

        [HttpPost("{stockId}")]
        public async Task<IActionResult> Create([FromBody] CreateCommentRequestDTO commentRequestDTO, [FromRoute] int stockId)
        {
            if (!await _stockRepo.StockExists(stockId))
            {
               return BadRequest("Stock does not exist");
            }

            var commentModel = _mapper.Map<Comment>(commentRequestDTO);
            commentModel.StockId = stockId; // Set the StockId for the comment
            await _commentRepo.CreateAsync(commentModel);
            return CreatedAtAction(nameof(GetById), new { id = commentModel.Id }, _mapper.Map<CommentDTO>(commentModel));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentRequestDTO commentRequestDTO)
        {
            var existingComment = await _commentRepo.GetByIdAsync(id);

            if (existingComment == null)
            {
                return NotFound();
            }

            _mapper.Map(commentRequestDTO, existingComment);
            await _commentRepo.UpdateAsync(id, commentRequestDTO);
            return Ok(_mapper.Map<CommentDTO>(existingComment));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var comment = await _commentRepo.GetByIdAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            await _commentRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}