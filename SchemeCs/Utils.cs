using System.Collections.Generic;

#nullable enable

namespace SchemeCs {
    public static class Utils {
        public static bool ListEquals<T>(List<T>? a, List<T>? b) {
            if (a == null && b == null) {
                return true;
            }

            if (a == null || b == null) {
                return false;
            }

            if (a.Count != b.Count) {
                return false;
            }

            for (int i = 0; i < a.Count; i++) {
                var ai = a[i];
                var bi = b[i];

                if (ai == null && bi == null) {
                    continue;
                }

                if (ai == null || bi == null) {
                    return false;
                }

                if (!ai.Equals(bi)) {
                    return false;
                }
            }

            return true;
        }
    }
}