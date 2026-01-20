
const { createClient } = require('@supabase/supabase-js');

const SUPABASE_URL = 'https://urtynghfmszehjwvligg.supabase.co';
const SUPABASE_ANON_KEY = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVydHluZ2hmbXN6ZWhqd3ZsaWdnIiwicm9sZSI6ImFub24iLCJpYXQiOjE3Njg0MDk2MDEsImV4cCI6MjA4Mzk4NTYwMX0.nLQd8gmmRH76HaYZ4kkOTbwTnm8EiOtdJ6GF9aHuEuA';

const supabase = createClient(SUPABASE_URL, SUPABASE_ANON_KEY, {
    auth: { persistSession: false }
});

const STUDENTS = [
    { name: 'Sophia Miller', age: 20, major: 'Psychology', gender: 'Female', photo: 'https://randomuser.me/api/portraits/women/44.jpg' },
    { name: 'Emma Wilson', age: 19, major: 'Biology', gender: 'Female', photo: 'https://randomuser.me/api/portraits/women/68.jpg' },
    { name: 'Olivia Davis', age: 21, major: 'Fine Arts', gender: 'Female', photo: 'https://randomuser.me/api/portraits/women/90.jpg' },
    { name: 'Liam Johnson', age: 20, major: 'Computer Science', gender: 'Male', photo: 'https://randomuser.me/api/portraits/men/32.jpg' },
    { name: 'Noah Brown', age: 22, major: 'Business', gender: 'Male', photo: 'https://randomuser.me/api/portraits/men/86.jpg' },
    { name: 'William Jones', age: 21, major: 'English Lit', gender: 'Male', photo: 'https://randomuser.me/api/portraits/men/11.jpg' },
];

async function seed() {
    console.log('--- Seeding Final V2 ---');

    // Get Max ID
    const { data: maxIdData } = await supabase
        .from('Students')
        .select('Id')
        .order('Id', { ascending: false })
        .limit(1);

    let nextId = (maxIdData && maxIdData.length > 0) ? maxIdData[0].Id + 1 : 1;
    console.log(`Starting ID: ${nextId}`);

    for (const s of STUDENTS) {
        const email = `seed_${Date.now()}_${s.name.split(' ')[0]}@test.com`;

        const { data, error } = await supabase
            .from('Students')
            .insert({
                Id: nextId, // Explicitly providing ID
                Email: email,
                Name: s.name,
                Age: s.age,
                Major: s.major,
                Gender: s.gender,
                Bio: 'Test Bio',
                PhotoUrl: s.photo,
                CreatedAt: new Date().toISOString(),
                // Omitting Photos array and University just in case
            })
            .select()
            .single();

        if (error) {
            console.error(`Error inserting ${s.name}: ${error.message}`);
        } else {
            console.log(`Success! Created ${s.name} (ID: ${data.Id})`);
            nextId++;
        }
    }
}

seed();
