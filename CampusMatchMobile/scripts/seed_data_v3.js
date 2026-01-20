
const { createClient } = require('@supabase/supabase-js');

// Configuration
const SUPABASE_URL = 'https://urtynghfmszehjwvligg.supabase.co';
const SUPABASE_ANON_KEY = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVydHluZ2hmbXN6ZWhqd3ZsaWdnIiwicm9sZSI6ImFub24iLCJpYXQiOjE3Njg0MDk2MDEsImV4cCI6MjA4Mzk4NTYwMX0.nLQd8gmmRH76HaYZ4kkOTbwTnm8EiOtdJ6GF9aHuEuA';

// Create a client with NO persistent session
const supabase = createClient(SUPABASE_URL, SUPABASE_ANON_KEY, {
    auth: {
        persistSession: false,
        autoRefreshToken: false,
        detectSessionInUrl: false
    }
});

const STUDENTS = [
    { email: 'sophia@test.com', name: 'Sophia Miller', age: 20, major: 'Psychology', bio: 'Coffee addict & future therapist ☕️', uni: 'CampusMatch U', gender: 'Female', photo: 'https://randomuser.me/api/portraits/women/44.jpg' },
    { email: 'emma@test.com', name: 'Emma Wilson', age: 19, major: 'Biology', bio: 'Nature lover 🌿 Hiking buddy needed!', uni: 'State College', gender: 'Female', photo: 'https://randomuser.me/api/portraits/women/68.jpg' },
    { email: 'olivia@test.com', name: 'Olivia Davis', age: 21, major: 'Fine Arts', bio: 'Always sketching. Let’s go to a museum! 🎨', uni: 'Arts Academy', gender: 'Female', photo: 'https://randomuser.me/api/portraits/women/90.jpg' },
    { email: 'liam@test.com', name: 'Liam Johnson', age: 20, major: 'Computer Science', bio: 'Tech enthusiast & gamer 🎮', uni: 'Tech Inst', gender: 'Male', photo: 'https://randomuser.me/api/portraits/men/32.jpg' },
    { email: 'noah@test.com', name: 'Noah Brown', age: 22, major: 'Business', bio: 'Crypto & chill 📈', uni: 'Business School', gender: 'Male', photo: 'https://randomuser.me/api/portraits/men/86.jpg' },
    { email: 'william@test.com', name: 'William Jones', age: 21, major: 'English Lit', bio: 'Bookworm looking for a study date 📚', uni: 'CampusMatch U', gender: 'Male', photo: 'https://randomuser.me/api/portraits/men/11.jpg' },
    { email: 'james@test.com', name: 'James Garcia', age: 19, major: 'Sports Management', bio: 'Always at the gym or the field 🏈', uni: 'State College', gender: 'Male', photo: 'https://randomuser.me/api/portraits/men/46.jpg' },
];

async function seed() {
    console.log('--- Starting Seeding (v3: Lowercase Table) ---');
    console.log('Using table: students');

    let createdCount = 0;

    for (const s of STUDENTS) {
        const uniqueEmail = `test_${Date.now()}_${s.email}`;
        const password = 'TestPassword123!';

        console.log(`\nRegistering ${s.name}...`);

        // 1. Sign Up
        const { data: authData, error: authError } = await supabase.auth.signUp({
            email: uniqueEmail,
            password: password,
        });

        if (authError) {
            console.error(`  Auth Error: ${authError.message}`);
            continue;
        }

        if (!authData.user) {
            console.error('  No user returned.');
            continue;
        }

        console.log(`  User Created (ID: ${authData.user.id}). Inserting Profile...`);

        // Wait a moment
        await new Promise(r => setTimeout(r, 200));

        // 2. Insert Profile (LOWERCASE TABLE NAME)
        // Using PascalCase columns logic as app code does: Email, Name, etc.
        // If table is lowercase, columns might still be PascalCase?
        // Or maybe exact column names are needed.
        // diag.js showed `Latitude: null` (Capital L). So columns are likely PascalCase.

        const { data: profile, error: profileError } = await supabase
            .from('students') // Lowercase table
            .insert({
                Email: uniqueEmail,
                Name: s.name,
                Age: s.age,
                Major: s.major,
                Bio: s.bio,
                Gender: s.gender,
                University: s.uni,
                Year: 'Junior',
                PhotoUrl: s.photo,
                Photos: [s.photo],
                CreatedAt: new Date().toISOString(),
            })
            .select()
            .single();

        if (profileError) {
            console.error(`  Profile Insert Error: ${profileError.message}`);
        } else {
            console.log(`  SUCCESS: Profile Created (ID: ${profile.Id || profile.id})`);
            createdCount++;
        }

        await supabase.auth.signOut();
    }

    console.log(`\n--- Seeding Complete. Created ${createdCount} profiles. ---`);
}

seed();
