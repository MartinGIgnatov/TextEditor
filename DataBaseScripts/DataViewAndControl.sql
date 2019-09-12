SELECT Word.[Name], WordType.[Name]
FROM ((Word_WordType
INNER JOIN Word ON Word_WordType.Id_Word = Word.Id)
INNER JOIN WordType ON Word_WordType.Id_WordType = WordType.Id);


SELECT * FROM Word where Id = 'b2a7fe79-f8bc-473b-92da-a315357be202';

SELECT * FROM WordType;

SELECT * FROM Word_WordType;