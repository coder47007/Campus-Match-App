
const { createClient } = require('@supabase/supabase-js');

const SUPABASE_URL = 'https://urtynghfmszehjwvligg.supabase.co';
const SUPABASE_ANON_KEY = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVydHluZ2hmbXN6ZWhqd3ZsaWdnIiwicm9sZSI6ImFub24iLCJpYXQiOjE3Njg0MDk2MDEsImV4cCI6MjA4Mzk4NTYwMX0.nLQd8gmmRH76HaYZ4kkOTbwTnm8EiOtdJ6GF9aHuEuA';

const supabase = createClient(SUPABASE_URL, SUPABASE_ANON_KEY, {
    auth: { persistSession: false }
});

const MOCK_USERS = [
    { name: 'Alice Walker', age: 21, gender: 'Female', photo: 'https://randomuser.me/api/portraits/women/55.jpg' },
    { name: 'Bob Smith', age: 22, gender: 'Male', photo: 'https://randomuser.me/api/portraits/men/55.jpg' },
    { name: 'Charlie Brown', age: 20, gender: 'Male', photo: 'https://randomuser.me/api/portraits/men/12.jpg' },
    { name: 'Diana Prince', age: 23, gender: 'Female', photo: 'https://randomuser.me/api/portraits/women/12.jpg' },
    { name: 'Evan Peters', age: 24, gender: 'Male', photo: 'https://randomuser.me/api/portraits/men/33.jpg' },
    { name: 'Fiona Gallagher', age: 21, gender: 'Female', photo: 'https://randomuser.me/api/portraits/women/33.jpg' },
    { name: 'George Michael', age: 22, gender: 'Male', photo: 'https://randomuser.me/api/portraits/men/44.jpg' },
    { name: 'Hannah Montana', age: 20, gender: 'Female', photo: 'https://randomuser.me/api/portraits/women/44.jpg' },
    { name: 'Ian Somerhalder', age: 25, gender: 'Male', photo: 'https://randomuser.me/api/portraits/men/22.jpg' },
    { name: 'Julia Roberts', age: 22, gender: 'Female', photo: 'https://randomuser.me/api/portraits/women/22.jpg' },
];

async function repair() {
    console.log('--- REPAIRING USERS ---');

    // 1. Get Top 10 IDs
    const { data: students, error } = await supabase
        .from('Students')
        .select('Id')
        .order('Id', { ascending: false })
        .limit(10);

    if (error || !students) {
        console.error('Error fetching students:', error);
        return;
    }

    console.log(`Found ${students.length} students to repair.`);

    // 2. Update them
    for (let i = 0; i < students.length; i++) {
        const student = students[i];
        const mock = MOCK_USERS[i % MOCK_USERS.length];

        console.log(`Updating ID ${student.Id} as ${mock.name}...`);

        const { error: updateError } = await supabase
            .from('Students')
            .update({
                Name: mock.name,
                Age: mock.age,
                Gender: mock.gender,
                PhotoUrl: mock.photo,
                Major: 'Undeclared',
                University: 'CampusMatch U',
                Bio: 'Repaired by Admin',
                // Ensure text[] columns are handled if possible, or omit if dangerous
                // Photos: [mock.photo] // Omit array to avoid schema cache error
            })
            .eq('Id', student.Id);

        if (updateError) {
            console.error(`  Failed: ${updateError.message}`);
        } else {
            console.log(`  Success!`);
        }
    }
}

repair();
