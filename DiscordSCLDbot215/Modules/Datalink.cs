using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace DiscordSCLDbot215.Modules
{
    class Datalink
    {

        readonly SQLiteAsyncConnection _database;

        public Datalink(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            /*
            _database.CreateTableAsync<Session_of_show>().Wait();
            _database.CreateTableAsync<Merchandises>().Wait();
            _database.CreateTableAsync<Orders>().Wait();
            _database.CreateTableAsync<Order_detail>().Wait();
            */
        }

        public void re_database()
        {
            /*
            _database.DropTableAsync<Session_of_show>().Wait();
            _database.DropTableAsync<Merchandises>().Wait();
            _database.DropTableAsync<Orders>().Wait();
            _database.DropTableAsync<Order_detail>().Wait();
            _database.CreateTableAsync<Session_of_show>().Wait();
            _database.CreateTableAsync<Merchandises>().Wait();
            _database.CreateTableAsync<Orders>().Wait();
            _database.CreateTableAsync<Order_detail>().Wait();
            */
        }

        #region 表演日的資料表
        /*
        public Task<List<Session_of_show>> Get_Session_of_shows_Async()
        {
            return _database.Table<Session_of_show>().ToListAsync();
        }

        public Task<Session_of_show> Get_Session_of_show_Async(int id)
        {
            return _database.Table<Session_of_show>()
                            .Where(i => i.session_id == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> Save_Session_of_show_Async(Session_of_show sos)
        {
            if (sos.session_id != 0)
            {
                return _database.UpdateAsync(sos);
            }
            else
            {   //是0的就跑這
                Insert_auto_Session_of_show(sos);
                sos.session_id = -99;
                return _database.UpdateAsync(sos);
            }
        }

        async void Insert_auto_Session_of_show(Session_of_show sos)
        {   //自動 +1 的部分
            Session_of_show sssss = await _database.Table<Session_of_show>().OrderByDescending(x => x.session_id).FirstOrDefaultAsync();

            if (sssss == null)
                sos.session_id = 1;
            else
                sos.session_id = sssss.session_id + 1;
            await _database.InsertAsync(sos);
        }

        public Task<int> Insert_Session_of_show_Async(Session_of_show sos)
        {
            //專們給匯入用的
            return _database.InsertAsync(sos);
        }

        public Task<int> Delete_Session_of_show_Async(Session_of_show note)
        {
            //連帶刪除
            delete_Session_link(note.session_id);
            return _database.DeleteAsync(note);
        }
        async void delete_Session_link(int the_session_id)
        {
            List<Orders> oooddd = (await App.Database.Get_Orders_Async()).FindAll(x => x.session_id == the_session_id);
            foreach (Orders todelete in oooddd)
            {
                await App.Database.Delete_Order_Async(todelete);
            }
        }

    */
        #endregion



    }
}
