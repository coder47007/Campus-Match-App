
const { createClient } = require('@supabase/supabase-js');

const SUPABASE_URL = 'https://urtynghfmszehjwvligg.supabase.co';
const SUPABASE_ANON_KEY = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVydHluZ2hmbXN6ZWhqd3ZsaWdnIiwicm9sZSI6ImFub24iLCJpYXQiOjE3Njg0MDk2MDEsImV4cCI6MjA4Mzk4NTYwMX0.nLQd8gmmRH76HaYZ4kkOTbwTnm8EiOtdJ6GF9aHuEuA';

const supabase = createClient(SUPABASE_URL, SUPABASE_ANON_KEY);

async function diagInsert() {
    console.log("Attempting insert into 'Students' (Capital)...");
    const { data, error } = await supabase
        .from('Students')
        .insert({
            Email: `diag_${Date.now()}@test.com`,
            Name: 'Diag User',
            CreatedAt: new Date().toISOString()
        })
        .select();

    if (error) {
        console.error('Insert Error:', error.message);
        console.error('Error Details:', error);
    } else {
        console.log('Insert Success:', data);
    }
}

diagInsert();
