/*
 Post-Deployment Script: seed all tables with initial data.
 This script is idempotent – it only inserts rows that do not already exist.
*/

-- =====================================================================
-- Address
-- =====================================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Address] WHERE [Id] = 1)
BEGIN
    INSERT INTO [dbo].[Address] ([Id], [Street],           [City],          [State],        [ZipCode])
    VALUES
        (1, '123 Main St',       'New York',      'New York',     '10001'),
        (2, '456 Oak Avenue',    'Los Angeles',   'California',   '90001'),
        (3, '789 Pine Road',     'Chicago',       'Illinois',     '60601'),
        (4, '321 Maple Drive',   'Houston',       'Texas',        '77001'),
        (5, '654 Elm Street',    'Phoenix',       'Arizona',      '85001');
END;

-- =====================================================================
-- Person
-- =====================================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Person] WHERE [Id] = 1)
BEGIN
    INSERT INTO [dbo].[Person] ([Id], [FirstName], [LastName])
    VALUES
        (1, 'Alice',   'Johnson'),
        (2, 'Bob',     'Williams'),
        (3, 'Carol',   'Brown'),
        (4, 'David',   'Jones'),
        (5, 'Eve',     'Garcia');
END;

-- =====================================================================
-- Company
-- =====================================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Company] WHERE [Id] = 1)
BEGIN
    INSERT INTO [dbo].[Company] ([Id], [Name],          [AddressId])
    VALUES
        (1, 'Acme Corp',     1),
        (2, 'Globex',        2),
        (3, 'Initech',       3),
        (4, 'Umbrella Co',   4),
        (5, 'Stark Ind',     5);
END;

-- =====================================================================
-- Employee
-- =====================================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Employee] WHERE [Id] = 1)
BEGIN
    INSERT INTO [dbo].[Employee] ([Id], [AddressId], [PersonId], [CompanyName],  [Position],          [EmployeeName])
    VALUES
        (1, 1, 1, 'Acme Corp',    'Software Engineer', 'Alice Johnson'),
        (2, 2, 2, 'Globex',       'Product Manager',   'Bob Williams'),
        (3, 3, 3, 'Initech',      'QA Analyst',        'Carol Brown'),
        (4, 4, 4, 'Umbrella Co',  'Team Lead',         'David Jones'),
        (5, 5, 5, 'Stark Ind',    'DevOps Engineer',   'Eve Garcia');
END;
