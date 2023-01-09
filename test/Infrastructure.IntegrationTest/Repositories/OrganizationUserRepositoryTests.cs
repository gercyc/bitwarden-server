﻿using Bit.Core.Entities;
using Bit.Core.Enums;
using Bit.Core.Repositories;
using Xunit;

namespace Bit.Infrastructure.IntegrationTest.Repositories;

public class OrganizationUserRepositoryTests
{
    [DatabaseTheory, DatabaseData]
    public async Task DeleteAsync_Works(IUserRepository userRepository,
        IOrganizationRepository organizationRepository,
        IOrganizationUserRepository organizationUserRepository,
        ITestDatabaseHelper helper)
    {
        var user = await userRepository.CreateAsync(new User
        {
            Name = "Test User",
            Email = "test@email.com",
            ApiKey = "TEST",
            SecurityStamp = "stamp",
        });

        var organization = await organizationRepository.CreateAsync(new Organization
        {
            Name = "Test Org",
        });

        var orgUser = await organizationUserRepository.CreateAsync(new OrganizationUser
        {
            OrganizationId = organization.Id,
            UserId = user.Id,
            Status = OrganizationUserStatusType.Confirmed,
        });

        helper.ClearTracker();

        await organizationUserRepository.DeleteAsync(orgUser);

        var newUser = await userRepository.GetByIdAsync(user.Id);
        Assert.NotEqual(newUser.AccountRevisionDate, user.AccountRevisionDate);
    }

    [DatabaseTheory, DatabaseData]
    public async Task DeleteManyAsync_Works(IUserRepository userRepository,
        IOrganizationRepository organizationRepository,
        IOrganizationUserRepository organizationUserRepository,
        ITestDatabaseHelper helper)
    {
        var user1 = await userRepository.CreateAsync(new User
        {
            Name = "Test User 1",
            Email = "test1@email.com",
            ApiKey = "TEST",
            SecurityStamp = "stamp",
        });

        var user2 = await userRepository.CreateAsync(new User
        {
            Name = "Test User 2",
            Email = "test1@email.com",
            ApiKey = "TEST",
            SecurityStamp = "stamp",
        });

        var organization = await organizationRepository.CreateAsync(new Organization
        {
            Name = "Test Org",
        });

        var orgUser1 = await organizationUserRepository.CreateAsync(new OrganizationUser
        {
            OrganizationId = organization.Id,
            UserId = user1.Id,
        });

        var orgUser2 = await organizationUserRepository.CreateAsync(new OrganizationUser
        {
            OrganizationId = organization.Id,
            UserId = user2.Id,
        });

        helper.ClearTracker();

        await organizationUserRepository.DeleteManyAsync(new List<Guid>
        {
            orgUser1.Id,
            orgUser2.Id,
        });

        var updatedUser1 = await userRepository.GetByIdAsync(user1.Id);
        var updatedUser2 = await userRepository.GetByIdAsync(user2.Id);

        Assert.NotEqual(updatedUser1.AccountRevisionDate, user1.AccountRevisionDate);
        Assert.NotEqual(updatedUser2.AccountRevisionDate, user2.AccountRevisionDate);
    }
}
