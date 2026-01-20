
const { createClient } = require('@supabase/supabase-js');

const SUPABASE_URL = 'https://urtynghfmszehjwvligg.supabase.co';
const SUPABASE_ANON_KEY = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVydHluZ2hmbXN6ZWhqd3ZsaWdnIiwicm9sZSI6ImFub24iLCJpYXQiOjE3Njg0MDk2MDEsImV4cCI6MjA4Mzk4NTYwMX0.nLQd8gmmRH76HaYZ4kkOTbwTnm8EiOtdJ6GF9aHuEuA';

const supabase = createClient(SUPABASE_URL, SUPABASE_ANON_KEY, {
    auth: { persistSession: false }
});

async function check() {
    console.log('--- Checking Students Table ---');

    // Count
    const { count, error } = await supabase
        .from('Students')
        .select('*', { count: 'exact', head: true });

    if (error) {
        console.error('Error counting:', error.message);
    } else {
        console.log(`Total Students: ${count}`);
    }

    // Get FULL Data for one student
    const { data } = await supabase
        .from('Students')
        .select('*')
        .order('Id', { ascending: false })
        .limit(1);

    console.log('Sample Student Full Data:', JSON.stringify(data, null, 2));
}

check();
