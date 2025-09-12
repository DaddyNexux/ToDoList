using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;
using ToDoList.Models.DTOs;
using ToDoList.Models.Entities;
using ToDoList.Data;
using Microsoft.EntityFrameworkCore;
using ToDoList.Models.DTOs.Common;

namespace ToDoList.Services
{
    public interface IToDoListService
    {
        Task<ApiResponse<ToDoListResponseDTO>> GetByIdAsync(Guid toDoId);
        Task<ApiResponse<List<ToDOList>>> GetAllWithUserAsync();
        Task<ApiResponse<List<ToDoListResponseDTO>>> GetByUserAsync(string token);
        Task<ApiResponse<ToDOList>> CreateAsync(string token, string title, string description);
        Task<ApiResponse<bool>> ToggleCompleteAsync(string token, Guid toDoId);
        Task<ApiResponse<bool>> UpdateAsync(string token, Guid toDoId, string title, string description);
        Task<ApiResponse<bool>> DeleteAsync(string token, Guid toDoId);
    }
    public class ToDoListService : IToDoListService
    {
        private readonly AppData _context;
        public ToDoListService(AppData context) => _context = context;

        // 📄 Get single by Id (no token required)
        public async Task<ApiResponse<ToDoListResponseDTO>> GetByIdAsync(Guid toDoId)
        {
            var entity = await _context.ToDOLists
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == toDoId);

            return entity == null
                ? ApiResponse<ToDoListResponseDTO>.Fail("To-Do not found", 404)
                : ApiResponse<ToDoListResponseDTO>.Success(MapToDto(entity));
        }

        // 📄 Get all including related User
        public async Task<ApiResponse<List<ToDOList>>> GetAllWithUserAsync()
        {
            var list = await _context.ToDOLists
                .Include(t => t.User)
                .AsNoTracking()
                .ToListAsync();

            return ApiResponse<List<ToDOList>>.Success(list);
        }

        // 📄 Get all items for the current user
        public async Task<ApiResponse<List<ToDoListResponseDTO>>> GetByUserAsync(string userId)
        {
            if (userId == string.Empty)
                return ApiResponse<List<ToDoListResponseDTO>>.Fail("Invalid or missing token", 401);

            var items = await _context.ToDOLists
                .AsNoTracking()
                .Where(t => t.UserId == userId)
                .ToListAsync();

            return ApiResponse<List<ToDoListResponseDTO>>.Success(items.Select(MapToDto).ToList());
        }

        // 📄 Create a new ToDo
        public async Task<ApiResponse<ToDOList>> CreateAsync(string userId, string title, string description)
        {
            if (userId == string.Empty)
                return ApiResponse<ToDOList>.Fail("Invalid or missing token", 401);

            var entity = new ToDOList
            {
                Title = title,
                Description = description,
                UserId = userId.ToString()
            };

            _context.ToDOLists.Add(entity);
            await _context.SaveChangesAsync();
            return ApiResponse<ToDOList>.Success(entity, "To-Do created", 201);
        }

        // 📄 Toggle completion
        public async Task<ApiResponse<bool>> ToggleCompleteAsync(string token, Guid toDoId)
        {
            var entity = await FindOwnedEntity(token, toDoId);
            if (entity == null)
                return ApiResponse<bool>.Fail("To-Do not found or not yours", 404);

            entity.IsCompleted = !entity.IsCompleted;
            entity.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return ApiResponse<bool>.Success(true, "Completion status toggled");
        }

        // 📄 Update title & description
        public async Task<ApiResponse<bool>> UpdateAsync(string token, Guid toDoId, string title, string description)
        {
            var entity = await FindOwnedEntity(token, toDoId);
            if (entity == null)
                return ApiResponse<bool>.Fail("To-Do not found or not yours", 404);

            entity.Title = title;
            entity.Description = description;
            entity.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.Success(true, "To-Do updated");
        }

        // 📄 Delete
        public async Task<ApiResponse<bool>> DeleteAsync(string token, Guid toDoId)
        {
            var entity = await FindOwnedEntity(token, toDoId);
            if (entity == null)
                return ApiResponse<bool>.Fail("To-Do not found or not yours", 404);

            _context.ToDOLists.Remove(entity);
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.Success(true, "To-Do deleted");
        }

        // ================= Helpers =================

        private static ToDoListResponseDTO MapToDto(ToDOList e) => new()
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            IsCompleted = e.IsCompleted
        };

      

        private async Task<ToDOList?> FindOwnedEntity(string userId, Guid toDoId)
        {
            if (userId == string.Empty) return null;

            return await _context.ToDOLists
                .FirstOrDefaultAsync(t => t.Id == toDoId && t.UserId == userId);
        }
    }
}
