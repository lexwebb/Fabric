namespace Fabric.Core {
    internal static class Extensions {
        public static string TrimStart(this string target, string trimChars) {
            if(target.StartsWith(trimChars)){
                var pos = trimChars.Length;
                return target.Substring(pos);
            }
            return target;
        }

        public static string TrimEnd(this string target, string trimChars) {
            if(target.EndsWith(trimChars)){
                var pos = target.IndexOf(trimChars);
                return target.Substring(0, pos);
            }
            return target;
        }
    }
}
