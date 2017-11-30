test=mytests
other_services=$(docker-compose ps | tail -n +3 | awk '{print $1}' | grep -vE "(_$test_)")
docker start $other_services; docker-compose start -i $test
