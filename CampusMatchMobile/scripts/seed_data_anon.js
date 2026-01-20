
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
    console.log('--- Starting Seeding (Anon Key, Full Data) ---');

    // 1. Check Max ID (just in case we need it later, or to see if table is empty)
    const { data: maxIdData, error: maxIdError } = await supabase
        .from('Students')
        .select('Id')
        .order('Id', { ascending: false })
        .limit(1);

    let nextId = 1;
    if (maxIdData && maxIdData.length > 0) {
        nextId = maxIdData[0].Id + 1;
        console.log(`Current Max ID: ${maxIdData[0].Id}. Next calculated ID: ${nextId}`);
    } else {
        console.log('Table appears empty or no read access (using ID 1 if needed).');
    }

    let createdCount = 0;

    for (const s of STUDENTS) {
        const uniqueEmail = `test_${Date.now()}_${s.email}`;

        console.log(`\nInserting ${s.name} (Email: ${uniqueEmail})...`);

        // Insert with Anon Key
        const { data: profile, error: profileError } = await supabase
            .from('Students')
            .insert({
                // Id: nextId, // Try WITHOUT Id first. If it fails, we uncomment this.
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
                LastActive: new Date().toISOString(),
                PhoneNumber: '',
                InstagramHandle: ''
            })
            .select()
            .single();

        if (profileError) {
            console.error(`  Insert Error: ${profileError.message}`);
            // Check if it's the ID issue
            if (profileError.message.includes('null value in column "Id"')) {
                console.log('  RETRYING with explicit ID...');
                // Retry with ID
                const { data: retryProfile, error: retryError } = await supabase
                    .from('Students')
                    .insert({
                        Id: nextId,
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
                        LastActive: new Date().toISOString(),
                        PhoneNumber: '',
                        InstagramHandle: ''
                    })
                    .select()
                    .single();

                if (retryError) {
                    console.error(`  Retry Failed: ${retryError.message}`);
                } else {
                    console.log(`  SUCCESS (Retry): Profile Created (ID: ${retryProfile.Id})`);
                    createdCount++;
                    nextId++;
                }
            }
        } else {
            console.log(`  SUCCESS: Profile Created (ID: ${profile.Id})`);
            createdCount++;
        }
    }

    console.log(`\n--- Seeding Complete. Created ${createdCount} profiles. ---`);
}

seed();
