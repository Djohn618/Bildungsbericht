using BildungsBericht.Models;
using System.Data;

namespace BildungsBericht.DB
{
    public class DBHelper: DBBase
    {
        public DBHelper( string iConnectionString ) : base( iConnectionString )
        {

        }

        public List<CLBenutzer> GetAllBenutzers()
        {
            List<CLBenutzer> ListBen = new List<CLBenutzer>();

            try
            {
                String query = "Select Id, Vorname, Nachname From tbl_benutzer";

                DataTable dtProtocolState = base.GetDataTable( query );

                foreach( DataRow row in dtProtocolState.Rows )
                {
                    int Id = Convert.ToInt32( row["Id"].ToString() );
                    String Vorname = row["Vorname"].ToString();
                    String Nachname = row["Nachname"].ToString();
                    ListBen.Add( new CLBenutzer { BenutzerId = Id, FirstName = Vorname, LastName = Nachname } );
                }

                return ListBen;
            }
            catch( Exception ex )
            {
                //Trace.HMI.Log( TL.L1, TT.E, TS.S, ex.Message );
                return null;
            }
        }

        // Einzelnen Benutzer nach ID abrufen
        public Benutzer GetBenutzerById(int id)
        {
            try
            {
                // ID Validierung - verhindert SQL Injection da ID ein Integer ist
                if (id <= 0)
                {
                    throw new ArgumentException("Ungültige Benutzer-ID");
                }

                String query = String.Format("SELECT Id, Vorname, Nachname, Geburtsdatum, Email, Rolle_Id, Lehrberuf_Id FROM tbl_benutzer WHERE Id = {0}", id);

                DataTable dt = base.GetDataTable(query);

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Benutzer
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        Vorname = row["Vorname"].ToString(),
                        Nachname = row["Nachname"].ToString(),
                        Geburtsdatum = row["Geburtsdatum"] != DBNull.Value ? Convert.ToDateTime(row["Geburtsdatum"]) : null,
                        Email = row["Email"] != DBNull.Value ? row["Email"].ToString() : null,
                        Passwort = "********", // Passwort wird nicht zurückgegeben aus Sicherheitsgründen
                        RolleId = Convert.ToInt32(row["Rolle_Id"]),
                        LehrberufId = row["Lehrberuf_Id"] != DBNull.Value ? (int?)Convert.ToInt32(row["Lehrberuf_Id"]) : null
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler beim Abrufen des Benutzers: {ex.Message}", ex);
            }
        }

        // Neuen Benutzer erstellen (Create)
        public int CreateBenutzer(Benutzer benutzer)
        {
            try
            {
                // SQL Query mit Parametern für sichere Dateneingabe
                String query = @"INSERT INTO tbl_benutzer (vorname, nachname, geburtsdatum, email, passwort, rolle_id, lehrberuf_id) 
                                VALUES (@vorname, @nachname, @geburtsdatum, @email, @passwort, @rolle_id, @lehrberuf_id);
                                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                // Parameter vorbereiten - verhindert SQL Injection
                var parameters = new System.Data.SqlClient.SqlParameter[]
                {
                    new System.Data.SqlClient.SqlParameter("@vorname", benutzer.Vorname),
                    new System.Data.SqlClient.SqlParameter("@nachname", benutzer.Nachname),
                    new System.Data.SqlClient.SqlParameter("@geburtsdatum", (object)benutzer.Geburtsdatum ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@email", (object)benutzer.Email ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@passwort", benutzer.Passwort),
                    new System.Data.SqlClient.SqlParameter("@rolle_id", benutzer.RolleId),
                    new System.Data.SqlClient.SqlParameter("@lehrberuf_id", (object)benutzer.LehrberufId ?? DBNull.Value)
                };

                // Transaktion starten
                base.BeginTransaction();
                // ExecuteScalarWithParameters verwenden, um die neue ID zu bekommen
                object result = base.ExecuteScalarWithParameters(query, parameters);
                // Transaktion abschließen
                base.Commit();

                // Neue ID zurückgeben oder 0 wenn fehlgeschlagen
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                // Bei Fehler: Transaktion rückgängig machen
                try { base.Rollback(); } catch { }
                throw new Exception($"Fehler beim Erstellen des Benutzers: {ex.Message}", ex);
            }
        }

        // Benutzer aktualisieren (Update)
        public bool UpdateBenutzer(Benutzer benutzer)
        {
            try
            {
                // Wenn Passwort der Platzhalter ist, nicht aktualisieren
                String query;
                System.Data.SqlClient.SqlParameter[] parameters;
                
                if (benutzer.Passwort == "********")
                {
                    // Query ohne Passwort-Update
                    query = @"UPDATE tbl_benutzer 
                            SET vorname = @vorname, 
                                nachname = @nachname, 
                                geburtsdatum = @geburtsdatum, 
                                email = @email, 
                                rolle_id = @rolle_id, 
                                lehrberuf_id = @lehrberuf_id 
                            WHERE id = @id";

                    parameters = new System.Data.SqlClient.SqlParameter[]
                    {
                        new System.Data.SqlClient.SqlParameter("@id", benutzer.Id),
                        new System.Data.SqlClient.SqlParameter("@vorname", benutzer.Vorname),
                        new System.Data.SqlClient.SqlParameter("@nachname", benutzer.Nachname),
                        new System.Data.SqlClient.SqlParameter("@geburtsdatum", (object)benutzer.Geburtsdatum ?? DBNull.Value),
                        new System.Data.SqlClient.SqlParameter("@email", (object)benutzer.Email ?? DBNull.Value),
                        new System.Data.SqlClient.SqlParameter("@rolle_id", benutzer.RolleId),
                        new System.Data.SqlClient.SqlParameter("@lehrberuf_id", (object)benutzer.LehrberufId ?? DBNull.Value)
                    };
                }
                else
                {
                    // Query mit Passwort-Update
                    query = @"UPDATE tbl_benutzer 
                            SET vorname = @vorname, 
                                nachname = @nachname, 
                                geburtsdatum = @geburtsdatum, 
                                email = @email, 
                                passwort = @passwort, 
                                rolle_id = @rolle_id, 
                                lehrberuf_id = @lehrberuf_id 
                            WHERE id = @id";

                    parameters = new System.Data.SqlClient.SqlParameter[]
                    {
                        new System.Data.SqlClient.SqlParameter("@id", benutzer.Id),
                        new System.Data.SqlClient.SqlParameter("@vorname", benutzer.Vorname),
                        new System.Data.SqlClient.SqlParameter("@nachname", benutzer.Nachname),
                        new System.Data.SqlClient.SqlParameter("@geburtsdatum", (object)benutzer.Geburtsdatum ?? DBNull.Value),
                        new System.Data.SqlClient.SqlParameter("@email", (object)benutzer.Email ?? DBNull.Value),
                        new System.Data.SqlClient.SqlParameter("@passwort", benutzer.Passwort),
                        new System.Data.SqlClient.SqlParameter("@rolle_id", benutzer.RolleId),
                        new System.Data.SqlClient.SqlParameter("@lehrberuf_id", (object)benutzer.LehrberufId ?? DBNull.Value)
                    };
                }

                // Query ausführen
                int rowsAffected = base.ExecuteSql(query, parameters);
                
                // Erfolgreich wenn mindestens eine Zeile geändert wurde
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler beim Aktualisieren des Benutzers: {ex.Message}", ex);
            }
        }

        // Benutzer löschen (Delete)
        public bool DeleteBenutzer(int benutzerId)
        {
            try
            {
                // SQL Query um Benutzer zu löschen
                String query = "DELETE FROM tbl_benutzer WHERE id = @id";

                // Parameter vorbereiten
                var parameters = new System.Data.SqlClient.SqlParameter[]
                {
                    new System.Data.SqlClient.SqlParameter("@id", benutzerId)
                };

                // Query ausführen
                int rowsAffected = base.ExecuteSql(query, parameters);
                
                // Erfolgreich wenn mindestens eine Zeile gelöscht wurde
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler beim Löschen des Benutzers: {ex.Message}", ex);
            }
        }

        public List<TemplateBericht> GetAllBerichte()
        {
            List<TemplateBericht> listBerichte = new List<TemplateBericht>();

            try
            {
                String query = @"SELECT id, lernender_id, berufsbildner_id, semester, berichtdatum, 
                                erstellt_durch_benutzer_id, tbl_lehrBeruf_id 
                                FROM tbl_template_bericht";

                DataTable dt = base.GetDataTable(query);

                foreach (DataRow row in dt.Rows)
                {
                    listBerichte.Add(new TemplateBericht
                    {
                        Id = Convert.ToInt32(row["id"]),
                        LernenderId = Convert.ToInt32(row["lernender_id"]),
                        BerufsbildnerId = row["berufsbildner_id"] != DBNull.Value ? Convert.ToInt32(row["berufsbildner_id"]) : null,
                        Semester = Convert.ToInt32(row["semester"]),
                        Berichtdatum = Convert.ToDateTime(row["berichtdatum"]),
                        ErstelltDurchBenutzerId = Convert.ToInt32(row["erstellt_durch_benutzer_id"]),
                        LehrberufId = Convert.ToInt32(row["tbl_lehrBeruf_id"])
                    });
                }

                return listBerichte;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler beim Laden der Berichte: {ex.Message}", ex);
            }
        }

        public int CreateBericht(TemplateBericht bericht)
        {
            try
            {
                String query = @"INSERT INTO tbl_template_bericht (lernender_id, berufsbildner_id, semester, berichtdatum, 
                                erstellt_durch_benutzer_id, tbl_lehrBeruf_id) 
                                VALUES (@lernender_id, @berufsbildner_id, @semester, @berichtdatum, @erstellt_durch_benutzer_id, @tbl_lehrBeruf_id);
                                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                var parameters = new System.Data.SqlClient.SqlParameter[]
                {
                    new System.Data.SqlClient.SqlParameter("@lernender_id", bericht.LernenderId),
                    new System.Data.SqlClient.SqlParameter("@berufsbildner_id", (object)bericht.BerufsbildnerId ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@semester", bericht.Semester),
                    new System.Data.SqlClient.SqlParameter("@berichtdatum", bericht.Berichtdatum),
                    new System.Data.SqlClient.SqlParameter("@erstellt_durch_benutzer_id", bericht.ErstelltDurchBenutzerId),
                    new System.Data.SqlClient.SqlParameter("@tbl_lehrBeruf_id", bericht.LehrberufId)
                };

                base.BeginTransaction();
                object result = base.ExecuteScalarWithParameters(query, parameters);
                base.Commit();
                
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                try { base.Rollback(); } catch { }
                throw new Exception($"Fehler beim Erstellen des Berichts: {ex.Message}", ex);
            }
        }

        // Bericht aktualisieren (Update)
        public bool UpdateBericht(TemplateBericht bericht)
        {
            try
            {
                // SQL Query um Bericht zu aktualisieren
                String query = @"UPDATE tbl_template_bericht 
                                SET lernender_id = @lernender_id, 
                                    berufsbildner_id = @berufsbildner_id, 
                                    semester = @semester, 
                                    berichtdatum = @berichtdatum, 
                                    erstellt_durch_benutzer_id = @erstellt_durch_benutzer_id, 
                                    tbl_lehrBeruf_id = @tbl_lehrBeruf_id 
                                WHERE id = @id";

                // Parameter vorbereiten
                var parameters = new System.Data.SqlClient.SqlParameter[]
                {
                    new System.Data.SqlClient.SqlParameter("@id", bericht.Id),
                    new System.Data.SqlClient.SqlParameter("@lernender_id", bericht.LernenderId),
                    new System.Data.SqlClient.SqlParameter("@berufsbildner_id", (object)bericht.BerufsbildnerId ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@semester", bericht.Semester),
                    new System.Data.SqlClient.SqlParameter("@berichtdatum", bericht.Berichtdatum),
                    new System.Data.SqlClient.SqlParameter("@erstellt_durch_benutzer_id", bericht.ErstelltDurchBenutzerId),
                    new System.Data.SqlClient.SqlParameter("@tbl_lehrBeruf_id", bericht.LehrberufId)
                };

                // Query ausführen
                int rowsAffected = base.ExecuteSql(query, parameters);
                
                // Erfolgreich wenn mindestens eine Zeile geändert wurde
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler beim Aktualisieren des Berichts: {ex.Message}", ex);
            }
        }

        // Bericht löschen (Delete)
        public bool DeleteBericht(int berichtId)
        {
            try
            {
                // SQL Query um Bericht zu löschen
                String query = "DELETE FROM tbl_template_bericht WHERE id = @id";

                // Parameter vorbereiten
                var parameters = new System.Data.SqlClient.SqlParameter[]
                {
                    new System.Data.SqlClient.SqlParameter("@id", berichtId)
                };

                // Query ausführen
                int rowsAffected = base.ExecuteSql(query, parameters);
                
                // Erfolgreich wenn mindestens eine Zeile gelöscht wurde
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler beim Löschen des Berichts: {ex.Message}", ex);
            }
        }

        public List<Selbstbewertung> GetAllSelbstbewertungen()
        {
            List<Selbstbewertung> list = new List<Selbstbewertung>();

            try
            {
                String query = @"SELECT id, selbst_note, reflexion, gelernt, herausforderungen, 
                                naechste_ziele, template_bericht_id 
                                FROM tbl_selbstbewertung_lernende";

                DataTable dt = base.GetDataTable(query);

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new Selbstbewertung
                    {
                        Id = Convert.ToInt32(row["id"]),
                        SelbstNote = row["selbst_note"] != DBNull.Value ? Convert.ToInt32(row["selbst_note"]) : null,
                        Reflexion = row["reflexion"] != DBNull.Value ? row["reflexion"].ToString() : null,
                        Gelernt = row["gelernt"] != DBNull.Value ? row["gelernt"].ToString() : null,
                        Herausforderungen = row["herausforderungen"] != DBNull.Value ? row["herausforderungen"].ToString() : null,
                        NaechsteZiele = row["naechste_ziele"] != DBNull.Value ? row["naechste_ziele"].ToString() : null,
                        TemplateBerichtId = Convert.ToInt32(row["template_bericht_id"])
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler beim Laden der Selbstbewertungen: {ex.Message}", ex);
            }
        }

        public int CreateSelbstbewertung(Selbstbewertung selbstbewertung)
        {
            try
            {
                String query = @"INSERT INTO tbl_selbstbewertung_lernende (selbst_note, reflexion, gelernt, herausforderungen, 
                                naechste_ziele, template_bericht_id) 
                                VALUES (@selbst_note, @reflexion, @gelernt, @herausforderungen, @naechste_ziele, @template_bericht_id);
                                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                var parameters = new System.Data.SqlClient.SqlParameter[]
                {
                    new System.Data.SqlClient.SqlParameter("@selbst_note", (object)selbstbewertung.SelbstNote ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@reflexion", (object)selbstbewertung.Reflexion ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@gelernt", (object)selbstbewertung.Gelernt ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@herausforderungen", (object)selbstbewertung.Herausforderungen ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@naechste_ziele", (object)selbstbewertung.NaechsteZiele ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@template_bericht_id", selbstbewertung.TemplateBerichtId)
                };

                base.BeginTransaction();
                object result = base.ExecuteScalarWithParameters(query, parameters);
                base.Commit();
                
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                try { base.Rollback(); } catch { }
                throw new Exception($"Fehler beim Erstellen der Selbstbewertung: {ex.Message}", ex);
            }
        }

        // Selbstbewertung aktualisieren (Update)
        public bool UpdateSelbstbewertung(Selbstbewertung selbstbewertung)
        {
            try
            {
                // SQL Query um Selbstbewertung zu aktualisieren
                String query = @"UPDATE tbl_selbstbewertung_lernende 
                                SET selbst_note = @selbst_note, 
                                    reflexion = @reflexion, 
                                    gelernt = @gelernt, 
                                    herausforderungen = @herausforderungen, 
                                    naechste_ziele = @naechste_ziele, 
                                    template_bericht_id = @template_bericht_id 
                                WHERE id = @id";

                // Parameter vorbereiten
                var parameters = new System.Data.SqlClient.SqlParameter[]
                {
                    new System.Data.SqlClient.SqlParameter("@id", selbstbewertung.Id),
                    new System.Data.SqlClient.SqlParameter("@selbst_note", (object)selbstbewertung.SelbstNote ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@reflexion", (object)selbstbewertung.Reflexion ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@gelernt", (object)selbstbewertung.Gelernt ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@herausforderungen", (object)selbstbewertung.Herausforderungen ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@naechste_ziele", (object)selbstbewertung.NaechsteZiele ?? DBNull.Value),
                    new System.Data.SqlClient.SqlParameter("@template_bericht_id", selbstbewertung.TemplateBerichtId)
                };

                // Query ausführen
                int rowsAffected = base.ExecuteSql(query, parameters);
                
                // Erfolgreich wenn mindestens eine Zeile geändert wurde
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler beim Aktualisieren der Selbstbewertung: {ex.Message}", ex);
            }
        }

        // Selbstbewertung löschen (Delete)
        public bool DeleteSelbstbewertung(int selbstbewertungId)
        {
            try
            {
                // SQL Query um Selbstbewertung zu löschen
                String query = "DELETE FROM tbl_selbstbewertung_lernende WHERE id = @id";

                // Parameter vorbereiten
                var parameters = new System.Data.SqlClient.SqlParameter[]
                {
                    new System.Data.SqlClient.SqlParameter("@id", selbstbewertungId)
                };

                // Query ausführen
                int rowsAffected = base.ExecuteSql(query, parameters);
                
                // Erfolgreich wenn mindestens eine Zeile gelöscht wurde
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler beim Löschen der Selbstbewertung: {ex.Message}", ex);
            }
        }
    }
}
