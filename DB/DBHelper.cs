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

        public int CreateBenutzer(Benutzer benutzer)
        {
            try
            {
                String query = "INSERT INTO tbl_benutzer (vorname, nachname, passwort) VALUES ('A', 'B', 'C' )";


                //String query = @"INSERT INTO tbl_benutzer (vorname, nachname, geburtsdatum, email, passwort, rolle_id, lehrberuf_id) 
                                //VALUES (@vorname, @nachname, @geburtsdatum, @email, @passwort, @rolle_id, @lehrberuf_id);
                                //SELECT CAST(SCOPE_IDENTITY() AS INT);";

                //var parameters = new System.Data.SqlClient.SqlParameter[]
                //{
                //    new System.Data.SqlClient.SqlParameter("@vorname", benutzer.Vorname),
                //    new System.Data.SqlClient.SqlParameter("@nachname", benutzer.Nachname),
                //    new System.Data.SqlClient.SqlParameter("@geburtsdatum", (object)benutzer.Geburtsdatum ?? DBNull.Value),
                //    new System.Data.SqlClient.SqlParameter("@email", (object)benutzer.Email ?? DBNull.Value),
                //    new System.Data.SqlClient.SqlParameter("@passwort", benutzer.Passwort),
                //    new System.Data.SqlClient.SqlParameter("@rolle_id", benutzer.RolleId),
                //    new System.Data.SqlClient.SqlParameter("@lehrberuf_id", (object)benutzer.LehrberufId ?? DBNull.Value)
                //};

                // Execute both INSERT and SELECT SCOPE_IDENTITY() in a single transaction
                base.BeginTransaction();
                object result = base.ExecuteSql(query, false);
                base.Commit();

                return 10;

                //return result != null ? Convert.ToInt32(result) : 0
                ;
            }
            catch (Exception ex)
            {
                try { base.Rollback(); } catch { }
                throw new Exception($"Fehler beim Erstellen des Benutzers: {ex.Message}", ex);
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
                object result = base.ExecuteScalar(query);
                base.Commit();
                
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                try { base.Rollback(); } catch { }
                throw new Exception($"Fehler beim Erstellen des Berichts: {ex.Message}", ex);
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
                object result = base.ExecuteScalar(query);
                base.Commit();
                
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                try { base.Rollback(); } catch { }
                throw new Exception($"Fehler beim Erstellen der Selbstbewertung: {ex.Message}", ex);
            }
        }
    }
}
