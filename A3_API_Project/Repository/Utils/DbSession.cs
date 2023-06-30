using A3_API_Project.Models.Cinema;
using A3_API_Project.Models.IDP;
using SQLite;
using System.IO;

namespace A3_API_Project.Repository.Utils
{
    public class DbSession
    {

        public readonly SQLiteAsyncConnection Connection;
        private static DbSession _database;
        private static string _diretorioBanco = Path.Combine($"{ Directory.GetCurrentDirectory() }\\Repository\\Utils", "cinema_db.db3");

        public DbSession()
        {
            Connection = new SQLiteAsyncConnection(_diretorioBanco, false);
            //DropTables();
            CreateTables();
        }

        // Create the database connection as a singleton.
        public DbSession Database
        {
            get
            {
                if (_database == null)
                {
                    _database = new DbSession();
                }
                return _database;
            }
        }

        public void CreateTables()
        {

            string queryRegiao = @"CREATE TABLE IF NOT EXISTS Regiao (
                                                                        ID                INTEGER PRIMARY KEY AUTOINCREMENT
                                                                                                  NOT NULL,
                                                                        Descricao    VARCHAR  NOT NULL
                                                                    );";



            string queryCidade = @"CREATE TABLE IF NOT EXISTS Cidade (
                                                        ID        INTEGER PRIMARY KEY AUTOINCREMENT
                                                                          NOT NULL,
                                                        Estado    VARCHAR,
                                                        Descricao VARCHAR NOT NULL
                                                    );
                                                    ";

            string queryCinema = @"CREATE TABLE IF NOT EXISTS Cinema (
                                                        ID                INTEGER PRIMARY KEY AUTOINCREMENT
                                                                                  NOT NULL,
                                                        AmrCidadeRegiaoId          INTEGER  NOT NULL,
                                                        NomeCinema        VARCHAR NOT NULL,
                                                        Endereco          VARCHAR,
                                                        Longitude         INTEGER,
                                                        Latitude          INTEGER,
                                                        NotaAvaliacao     INTEGER,
                                                        FOREIGN KEY (
                                                            AmrCidadeRegiaoId
                                                        )
                                                        REFERENCES AmrCidadeRegiaoId (Id) 
                                                    );";


            // CinemaId          INTEGER NOT NULL,
            string queryFilme = @"CREATE TABLE IF NOT EXISTS Filme (
                                                        ID            INTEGER PRIMARY KEY AUTOINCREMENT
                                                                              NOT NULL,
                                                        NomeFilme     VARCHAR NOT NULL,
                                                        Sinopse       VARCHAR NOT NULL,
                                                        Diretor       VARCHAR NOT NULL,
                                                        Idioma        VARCHAR NOT NULL,
                                                        Duracao       INTEGER NOT NULL,
                                                        Categoria     VARCHAR NOT NULL,
                                                        NotaAvaliacao INTEGER,
                                                        FotoCapaFilme BLOB,
                                                        FaixaEtaria   INTEGER
                                                    );";

            string querySessao = @"CREATE TABLE IF NOT EXISTS Sessao (
                                                            ID            INTEGER  PRIMARY KEY AUTOINCREMENT
                                                                                   NOT NULL,
                                                            FilmeCinemaId INTEGER  NOT NULL,
                                                            DataHora      DATETIME NOT NULL,
                                                            FOREIGN KEY (
                                                                FilmeCinemaId
                                                            )
                                                            REFERENCES FilmeCinema (Id),
                                                            CONSTRAINT sessao_filmeCinemaId_unico UNIQUE (
                                                                FilmeCinemaId,
                                                                DataHora
                                                            )
                                                        );";

            string queryFilmeCinemaAmarracao = @"CREATE TABLE IF NOT EXISTS Amr_FilmeCinema (
                                                    Id       INTEGER NOT NULL
                                                                     PRIMARY KEY AUTOINCREMENT,
                                                    CinemaId INTEGER NOT NULL,
                                                    FilmeId  INTEGER NOT NULL,
                                                    CONSTRAINT chave_composta_filme_cinema UNIQUE (
                                                        FilmeId,
                                                        CinemaId
                                                    ),
                                                    FOREIGN KEY (
                                                        FilmeId
                                                    )
                                                    REFERENCES Filme (ID),
                                                    FOREIGN KEY (
                                                        CinemaId
                                                    )
                                                    REFERENCES Cinema (ID) 
                                                );";


            string queryFotosCinema = @"CREATE TABLE IF NOT EXISTS FotoCinema (
                                            Id       INTEGER PRIMARY KEY AUTOINCREMENT
                                                             NOT NULL,
                                            CinemaId INTEGER NOT NULL,
                                            Foto     BLOB    NOT NULL,
                                            FOREIGN KEY (
                                                CinemaId
                                            )
                                            REFERENCES Cinema (ID) 
                                        );";

            string queryAmarracaoCidadeRegiao = @"CREATE TABLE IF NOT EXISTS Amr_CidadeRegiao (
                                                        Id       INTEGER PRIMARY KEY AUTOINCREMENT,
                                                        RegiaoId INTEGER NOT NULL,
                                                        CidadeId INTEGER NOT NULL,
                                                        FOREIGN KEY (
                                                            RegiaoId
                                                        )
                                                        REFERENCES Regiao (ID),
                                                        FOREIGN KEY (
                                                            CidadeId
                                                        )
                                                        REFERENCES Cidade (ID),
                                                        CONSTRAINT Unique_Constraint UNIQUE (
                                                            RegiaoId,
                                                            CidadeId
                                                        )
                                                    );";

            string queryUsuario = @"CREATE TABLE IF NOT EXISTS Usuario (
                                            Id       INTEGER PRIMARY KEY AUTOINCREMENT,
                                            Username VARCHAR NOT NULL
                                                             UNIQUE,
                                            Senha    VARCHAR NOT NULL
                                        );";

            string queryPerfil = @"CREATE TABLE IF NOT EXISTS Perfil (
                                                    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
                                                    Descricao VARCHAR UNIQUE
                                                                      NOT NULL
                                                );";

            string queryAmrPerfilUsuario = @"CREATE TABLE IF NOT EXISTS Amr_PerfilUsuario (
                                                    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
                                                    UsuarioId INTEGER NOT NULL,
                                                    PerfilId  INTEGER NOT NULL,
                                                    FOREIGN KEY (
                                                        UsuarioId
                                                    )
                                                    REFERENCES Usuario (Id),
                                                    FOREIGN KEY (
                                                        PerfilId
                                                    )
                                                    REFERENCES Perfil (Id),
                                                    CONSTRAINT unique_constraint UNIQUE (
                                                        UsuarioId,
                                                        PerfilId
                                                    )
                                                );";

            string roleTable = @"CREATE TABLE IF NOT EXISTS ApplicationRole (
                                        Id             INTEGER NOT NULL 
                                                            CONSTRAINT PK_Roles PRIMARY KEY AUTOINCREMENT,
                                        Name           TEXT NOT NULL UNIQUE,
                                        NormalizedName TEXT NOT NULL UNIQUE
                                    );
                                    ";

            string userTable = @"CREATE TABLE IF NOT EXISTS ApplicationUser (
                                        Id                   INTEGER NOT NULL 
                                                                     CONSTRAINT PK_Users PRIMARY KEY AUTOINCREMENT,
                                        UserName             TEXT    NOT NULL,
                                        NormalizedUserName   TEXT    NOT NULL,
                                        Email                TEXT,
                                        NormalizedEmail      TEXT,
                                        EmailConfirmed       INTEGER NOT NULL,
                                        PasswordHash         TEXT,
                                        PhoneNumber          TEXT,
                                        PhoneNumberConfirmed INTEGER NOT NULL,
                                        TwoFactorEnabled     INTEGER NOT NULL
                                    );";

            string userRoleTable = @"CREATE TABLE IF NOT EXISTS ApplicationUserRole (
                                                UserId INTEGER NOT NULL,
                                                RoleId INTEGER NOT NULL,
                                                CONSTRAINT PK_UserRoles PRIMARY KEY (
                                                    UserId,
                                                    RoleId
                                                ),
                                                CONSTRAINT FK_UserRoles_Roles_RoleId FOREIGN KEY (
                                                    RoleId
                                                )
                                                REFERENCES ApplicationRole (Id) ON DELETE CASCADE
                                                MATCH [FULL],
                                                CONSTRAINT FK_UserRoles_Users_UserId FOREIGN KEY (
                                                    UserId
                                                )
                                                REFERENCES ApplicationUser (Id) ON DELETE CASCADE
                                                MATCH [FULL]
                                            );
                                            ";

            // gera tabelas COM FKs
            Connection.QueryAsync<Regiao>(queryRegiao).Wait();
            Connection.QueryAsync<Cidade>(queryCidade).Wait();
            Connection.QueryAsync<Cinema>(queryCinema).Wait();
            Connection.QueryAsync<Filme>(queryFilme).Wait();
            Connection.QueryAsync<Sessao>(querySessao).Wait();
            Connection.QueryAsync<AmarracaoFilmeCinema>(queryFilmeCinemaAmarracao).Wait();
            Connection.QueryAsync<FotoCinema>(queryFotosCinema).Wait();
            Connection.QueryAsync<AmarracaoCidadeRegiao>(queryAmarracaoCidadeRegiao).Wait();
            Connection.QueryAsync<Usuario>(queryUsuario).Wait();
            Connection.QueryAsync<Perfil>(queryPerfil).Wait();
            Connection.QueryAsync<AmarracaoPerfilUsuario>(queryAmrPerfilUsuario).Wait();
            Connection.QueryAsync<ApplicationUser>(roleTable).Wait();
            Connection.QueryAsync<ApplicationRole>(userTable).Wait();
            Connection.QueryAsync<ApplicationUserRole>(userRoleTable).Wait();


            // gera tabelas SEM RELACIONAMENTOS/FKs
            //Connection.CreateTablesAsync<Model_1, Model2, Model_3>().Wait();

        }

        public void DropTables()
        {
            Connection.DropTableAsync<AmarracaoCidadeRegiao>().Wait();
            Connection.DropTableAsync<AmarracaoFilmeCinema>().Wait();
            Connection.DropTableAsync<AmarracaoPerfilUsuario>().Wait();
            Connection.DropTableAsync<Regiao>().Wait();
            Connection.DropTableAsync<Cidade>().Wait();
            Connection.DropTableAsync<Cinema>().Wait();
            Connection.DropTableAsync<Filme>().Wait();
            Connection.DropTableAsync<Sessao>().Wait();
            Connection.DropTableAsync<Usuario>().Wait();
            Connection.DropTableAsync<Perfil>().Wait();
            Connection.DropTableAsync<ApplicationUserRole>().Wait();
            Connection.DropTableAsync<ApplicationRole>().Wait();
            Connection.DropTableAsync<ApplicationUser>().Wait();
        }

    }
}
