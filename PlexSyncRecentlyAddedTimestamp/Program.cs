using PlexSyncRecentlyAddedTimestamp.Repositories;
using System.Data;

//full path to your source database
var SourceDB = "com.plexapp.plugins.library.db";
//full path to your destination database
var DestinationDB = "com.plexapp.plugins.library.db";

var dt = SqliteRepository.ReadSQLiteData(SourceDB, "select media_items.created_at, metadata_items.guid  from media_items inner join media_parts on media_items.id = media_parts.media_item_id inner join metadata_items on metadata_items.id = media_items.metadata_item_id");
var ids = SqliteRepository.ReadSQLiteData(DestinationDB, "select id,guid from metadata_items");

int itemCounter = 0;
foreach (DataRow row in dt.Rows)
{
    itemCounter++;
    if (itemCounter % 100 == 0)
        Console.WriteLine($"Processing item {itemCounter} / {dt.Rows.Count}");
    string guid = row.Field<string>("guid")!;

    if (!row.IsNull("created_at"))
    {
        Int64 createdAt = row.Field<Int64>("created_at");
        var createdAtDT = DateTimeOffset.FromUnixTimeSeconds(createdAt);

        var r = ids.Select($"guid='{guid.Replace("'", "''")}'").FirstOrDefault();
        if (r != null)
        {
            Int64? destMediaId = r.Field<Int64>("id");
            if (destMediaId.HasValue)
                SqliteRepository.UpdateSQLiteData(DestinationDB, $"update media_items set created_at='{createdAtDT}' where id='{destMediaId}'");
        }
    }
}