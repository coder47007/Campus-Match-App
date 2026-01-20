
const { createClient } = require('@supabase/supabase-js');

const SUPABASE_URL = 'https://urtynghfmszehjwvligg.supabase.co';
const SUPABASE_ANON_KEY = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVydHluZ2hmbXN6ZWhqd3ZsaWdnIiwicm9sZSI6ImFub24iLCJpYXQiOjE3Njg0MDk2MDEsImV4cCI6MjA4Mzk4NTYwMX0.nLQd8gmmRH76HaYZ4kkOTbwTnm8EiOtdJ6GF9aHuEuA';

const supabase = createClient(SUPABASE_URL, SUPABASE_ANON_KEY, {
    auth: { persistSession: false }
});

const STUDENTS = [
    { name: 'Sarah Connor', age: 21, major: 'Cybersecurity', gender: 'Female', photo: 'https://randomuser.me/api/portraits/women/65.jpg', bio: 'Looking for a study partner 🔒' },
    { name: 'Nathan Drake', age: 23, major: 'History', gender: 'Male', photo: 'https://randomuser.me/api/portraits/men/1.jpg', bio: 'Thrill seeker 🗺️' },
    { name: 'Jessica Day', age: 22, major: 'Education', gender: 'Female', photo: 'https://randomuser.me/api/portraits/women/42.jpg', bio: 'New girl in town 🌟' },
    { name: 'Lucas Scott', age: 20, major: 'Literature', gender: 'Male', photo: 'https://randomuser.me/api/portraits/men/33.jpg', bio: 'Writer and basketball player 🏀' },
];

async function seed() {
    console.log('--- Seeding NO ARRAY ---');

    const { data: maxIdData } = await supabase.from('Students').select('Id').order('Id', { ascending: false }).limit(1);
    let nextId = (maxIdData && maxIdData.length > 0) ? maxIdData[0].Id + 1 : 2000;

    console.log(`Starting ID: ${nextId}`);

    for (const s of STUDENTS) {
        const email = `seed_no_arr_${Date.now()}_${s.name.split(' ')[0]}@test.com`;

        // Omit 'Photos', 'Interests', 'Prompts' arrays
        const { data, error } = await supabase
            .from('Students')
            .insert({
                Id: nextId,
                Email: email,
                Name: s.name,
                Age: s.age,
                Major: s.major,
                Gender: s.gender,
                Bio: s.bio,
                University: 'CampusMatch U',
                Year: 'Senior',
                PhotoUrl: s.photo,
                PhoneNumber: '555-0000',
                InstagramHandle: '@test',
                LastActive: new Date().toISOString(),
                CreatedAt: new Date().toISOString()
            })
            .select()
            .single();

        if (error) {
            console.error(`FAILED ${s.name}: ${error.message}`);
        } else {
            console.log(`SUCCESS: ${s.name} (ID: ${data.Id})`);
            nextId++;
        }
    }
}

seed();
