using Capstoners2026.Web.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstoners2026.Tests
{
    public class ApplicationUserTests
    {
        private AppDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        public ApplicationUserTests(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager; 
        }

        [Fact]
        public async Task NewUserHasZeroGrants()
        {

            ApplicationUser testUser = new ApplicationUser();
            testUser.UserName = "abc123@email.com";
            testUser.Email = "abc123@email.com";
            await _userManager.CreateAsync(testUser, "TestPassword123%");

            Assert.False(_context.Grants.Any(g => g.ProjectDirectorId == testUser.Id));

            await _userManager.DeleteAsync(testUser);

        }

        [Fact]
        public async Task NewUserIsNotDeanOfAnyCollege()
        {
            ApplicationUser testUser = new ApplicationUser();
            testUser.UserName = "abc123@email.com";
            testUser.Email = "abc123@email.com";
            await _userManager.CreateAsync(testUser, "TestPassword123%");

            Assert.False(_context.Colleges.Any(g => g.DeanId == testUser.Id));

            await _userManager.DeleteAsync(testUser);

        }

        [Fact]
        public async Task NewUserIsNotChairOfAnyDepartment()
        {

            ApplicationUser testUser = new ApplicationUser();
            testUser.UserName = "abc123@email.com";
            testUser.Email = "abc123@email.com";
            await _userManager.CreateAsync(testUser, "TestPassword123%");

            Assert.False(_context.Departments.Any(g => g.ChairId == testUser.Id));

            await _userManager.DeleteAsync(testUser);
        }

        [Fact]
        public async Task NewUserIsNotInCommittee()
        {
            ApplicationUser testUser = new ApplicationUser();
            testUser.UserName = "abc123@email.com";
            testUser.Email = "abc123@email.com";
            await _userManager.CreateAsync(testUser, "TestPassword123%");

            var isCommittee = await _userManager.IsInRoleAsync(testUser, "Committee");
            Assert.False(isCommittee);

            await _userManager.DeleteAsync(testUser);

        }

        [Fact]
        public async Task NewUserIsNotCommitteeChair()
        {
            ApplicationUser testUser = new ApplicationUser();
            testUser.UserName = "abc123@email.com";
            testUser.Email = "abc123@email.com";
            await _userManager.CreateAsync(testUser, "TestPassword123%");

            var isCommittee = await _userManager.IsInRoleAsync(testUser, "CommitteeChair");
            Assert.False(isCommittee);

            await _userManager.DeleteAsync(testUser);
        }

    }
}
