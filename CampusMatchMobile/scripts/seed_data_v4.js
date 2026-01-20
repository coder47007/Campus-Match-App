
const { createClient } = require('@supabase/supabase-js');

const SUPABASE_URL = 'https://urtynghfmszehjwvligg.supabase.co';
const SUPABASE_ANON_KEY = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVydHluZ2hmbXN6ZWhqd3ZsaWdnIiwicm9sZSI6ImFub24iLCJpYXQiOjE3Njg0MDk2MDEsImV4cCI6MjA4Mzk4NTYwMX0.nLQd8gmmRH76HaYZ4kkOTbwTnm8EiOtdJ6GF9aHuEuA';

const supabase = createClient(SUPABASE_URL, SUPABASE_ANON_KEY, {
    auth: { persistSession: false }
});

const STUDENTS = [
    { name: 'Sophia Miller', age: 20, major: 'Psychology', gender: 'Female', photo: 'https://randomuser.me/api/portraits/women/44.jpg' },
    { name: 'Emma Wilson', age: 19, major: 'Biology', gender: 'Female', photo: 'https://randomuser.me/api/portraits/women/68.jpg' },
    { name: 'Liam Johnson', age: 20, major: 'Computer Science', gender: 'Male', photo: 'https://randomuser.me/api/portraits/men/32.jpg' },
    { name: 'Noah Brown', age: 22, major: 'Business', gender: 'Male', photo: 'https://randomuser.me/api/portraits/men/86.jpg' },
];

async function seed() {
    console.log('--- Seeding V4 (No ID, No Photos Array) ---');
    console.log('Using Anon Key');

    for (const s of STUDENTS) {
        const email = `seedv4_${Date.now()}_${s.name.split(' ')[0]}@test.com`;

        const { data, error } = await supabase
            .from('Students')
            .insert({
                // No Id
                Email: email,
                Name: s.name,
                Age: s.age,
                Major: s.major,
                Gender: s.gender,
                Bio: 'Test Bio V4',
                University: 'CampusMatch U',
                Year: 'Junior',
                PhotoUrl: s.photo,

                PhoneNumber: '555-0100', // Dummy
                InstagramHandle: '@test', // Dummy
                CreatedAt: new Date().toISOString(),
                LastActive: new Date().toISOString()
                // No Photos array
            })
            .select()
            .single();

        if (error) {
            console.error(`Error inserting ${s.name}: ${error.message}`);
        } else {
            console.log(`Success! Created ${s.name} (ID: ${data.Id})`);
        }
    }
}

seed();
