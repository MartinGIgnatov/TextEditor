CREATE TABLE [WordType](
    [Id] [uniqueidentifier] NOT NULL,
    [Name] [nvarchar](50) NULL,
    PRIMARY KEY (Id)
)

ALTER TABLE [WordType]
ADD UNIQUE ([Name]);

INSERT INTO [WordType] (Id, Name)
VALUES (newid(), 'Noun');
INSERT INTO [WordType] (Id, Name)
VALUES (newid(), 'NounPlural');
INSERT INTO [WordType] (Id, Name)
VALUES (newid(), 'VerbalNoun');
INSERT INTO [WordType] (Id, Name)
VALUES (newid(), 'Adjective');
INSERT INTO [WordType] (Id, Name)
VALUES (newid(), 'Adverb');
INSERT INTO [WordType] (Id, Name)
VALUES (newid(), 'PresentParticip');
INSERT INTO [WordType] (Id, Name)
VALUES (newid(), 'PastParticip');
INSERT INTO [WordType] (Id, Name)
VALUES (newid(), 'VerbTransitive');
INSERT INTO [WordType] (Id, Name)
VALUES (newid(), 'VerbIntransitive');
INSERT INTO [WordType] (Id, Name)
VALUES (newid(), 'Imperative');
INSERT INTO [WordType] (Id, Name)
VALUES (newid(), 'Interjection');
INSERT INTO [WordType] (Id, Name)
VALUES (newid(), 'Specific');
INSERT INTO [WordType] (Id, Name)
VALUES (newid(), 'NotFound');
INSERT INTO [WordType] (Id, Name)
VALUES (newid(), 'Preposition');
INSERT INTO [WordType] (Id, Name)
VALUES (NEWID(), 'Infinitive');
INSERT INTO [WordType] (Id, Name)
VALUES (NEWID(), 'Pronoun');
INSERT INTO [WordType] (Id, Name)
VALUES (NEWID(), 'Prefix');

