using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ReadingFilesAndCreatingWordObjects
{
    public class InserterWordToDatabase
    {
        public void Insert(List<WordStruct> words)
        {
            string queryInsertWords = "INSERT INTO Word VALUES( @Id , @Name , @Description ) ";
            string queryInsertWord_WordTypes = "INSERT INTO Word_WordType VALUES( @Id_Word , @Id_WordType) ";
            string querySelectWordType = "SELECT * FROM WordType";
            var connectionString = "Data Source=localhost;Initial Catalog=TextEditor;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                Guid wordId;
                var parameter = new DynamicParameters();

                var wordTypes = connection.Query<WordTypeModel>(querySelectWordType).ToList();
                ;
                foreach (WordStruct word in words)
                {
                    ;
                    parameter = new DynamicParameters();
                    wordId = Guid.NewGuid();
                    parameter.Add("@Id", wordId, DbType.Guid, ParameterDirection.Input);
                    parameter.Add("@Name", word.Word, DbType.String, ParameterDirection.Input);
                    parameter.Add("@Description", word.Description, DbType.String, ParameterDirection.Input);

                    connection.Execute(queryInsertWords, parameter);
                    ;
                    foreach(WordTypes wordType in word.Types)
                    {
                        string wordTypeName = Enum.GetName(typeof(WordTypes), wordType);
                        Guid id_WordType = wordTypes.FirstOrDefault(type => type.Name == wordTypeName).Id;

                        parameter = new DynamicParameters();
                        parameter.Add("@Id_Word", wordId, DbType.Guid, ParameterDirection.Input);
                        parameter.Add("@Id_WordType", id_WordType, DbType.Guid, ParameterDirection.Input);
                        connection.Execute(queryInsertWord_WordTypes, parameter);
                    }
                }
            }
        }
    }
}
