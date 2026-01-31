export default ({ config }) => {
    return {
        ...config,
        name: config.name || "CampusMatchMobile",
        slug: config.slug || "CampusMatchMobile",
        extra: {
            apiUrl: process.env.API_URL,
            supabaseUrl: process.env.SUPABASE_URL,
            supabaseAnonKey: process.env.SUPABASE_ANON_KEY,
            eas: {
                projectId: "e7fe114e-d6d7-4d74-a58e-9ac5a73d4c14"
            }
        },
        plugins: [
            "expo-router",
            "expo-secure-store"
        ]
    };
};
