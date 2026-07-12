using System.Text.Json;

namespace MilkTea.Web.TienIch
{
    public static class TienIchSession
    {
        public static void DatDoiTuong<T>(
            this ISession session,
            string khoa,
            T giaTri)
        {
            string duLieuJson = JsonSerializer.Serialize(giaTri);

            session.SetString(khoa, duLieuJson);
        }

        public static T? LayDoiTuong<T>(
            this ISession session,
            string khoa)
        {
            string? duLieuJson = session.GetString(khoa);

            if (string.IsNullOrWhiteSpace(duLieuJson))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(duLieuJson);
        }
    }
}